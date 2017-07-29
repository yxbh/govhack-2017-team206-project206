package com.graphhopper.examples;

import com.google.inject.AbstractModule;
import com.google.inject.Guice;
import com.google.inject.Injector;
import com.google.inject.Module;
import com.google.inject.servlet.GuiceFilter;
import com.graphhopper.GraphHopper;
import com.graphhopper.http.DefaultModule;
import com.graphhopper.http.GHServer;
import com.graphhopper.http.GHServletModule;
import com.graphhopper.routing.util.EdgeFilter;
import com.graphhopper.routing.util.FastestWeighting;
import com.graphhopper.routing.util.FlagEncoder;
import com.graphhopper.routing.util.Weighting;
import com.graphhopper.routing.util.WeightingMap;
import com.graphhopper.storage.index.LocationIndex;
import com.graphhopper.storage.index.QueryResult;
import com.graphhopper.util.CmdArgs;
import com.graphhopper.util.EdgeExplorer;
import com.graphhopper.util.EdgeIterator;
import com.graphhopper.util.EdgeIteratorState;
import com.graphhopper.util.Helper;
import com.graphhopper.util.PMap;
import gnu.trove.set.hash.TIntHashSet;
import java.io.File;
import java.io.FileInputStream;
import org.json.JSONArray;
import org.json.JSONObject;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

/**
 * Get PBF from http://download.bbbike.org/osm/bbbike/Canberra/
 *
 * @author Peter Karich
 */
public class Canberra {

    public static void main(String[] args) throws Exception {
        new Canberra().start(CmdArgs.read(args));
    }

    private final Logger logger = LoggerFactory.getLogger(getClass());
    private CmdArgs args;

    public void start(CmdArgs tmpArgs) throws Exception {
        args = CmdArgs.readFromConfigAndMerge(tmpArgs, "config", "graphhopper.config");
        GHServer server = new GHServer(args);
        Injector injector = Guice.createInjector(createModule());
        server.start(injector);
    }

    static class AvoidCameraWeighting extends FastestWeighting {

        private final TIntHashSet avoidEdgeIds;

        public AvoidCameraWeighting(TIntHashSet avoidEdgeIds, FlagEncoder encoder, PMap pMap) {
            super(encoder, pMap);
            this.avoidEdgeIds = avoidEdgeIds;
        }

        @Override
        public double calcWeight(EdgeIteratorState edgeState, boolean reverse, int prevOrNextEdgeId) {
            double w = super.calcWeight(edgeState, reverse, prevOrNextEdgeId);

            if (avoidEdgeIds.contains(edgeState.getEdge())) {
                return w * 1000; //TODO instead of 1000, times by the SumPenAmt !!!
            } else {
                return w;
            }
        }
    }

    protected Module createModule() {
        return new AbstractModule() {
            @Override
            protected void configure() {
                binder().requireExplicitBindings();

                install(new DefaultModule(args) {

                    @Override
                    protected GraphHopper createGraphHopper(CmdArgs args) {
                        final TIntHashSet avoidEdgeIds = new TIntHashSet();

                        GraphHopper tmpHopper = new GraphHopper() {

                            @Override
                            public Weighting createWeighting(WeightingMap weightingMap, FlagEncoder encoder) {
                                String weighting = weightingMap.getWeighting();
                                if ("avoidcamera".equalsIgnoreCase(weighting)) {
                                    return new AvoidCameraWeighting(avoidEdgeIds, encoder, weightingMap);
                                }
                                return super.createWeighting(weightingMap, encoder);
                            }
                        }.forServer().init(args);
                        tmpHopper.importOrLoad();
                        LocationIndex index = tmpHopper.getLocationIndex();
                        logger.info("loaded graph at:" + tmpHopper.getGraphHopperLocation()
                                + ", source:" + tmpHopper.getOSMFile()
                                + ", flagEncoders:" + tmpHopper.getEncodingManager()
                                + ", class:" + tmpHopper.getGraphHopperStorage().toDetailsString());

                        // TODO LATER make persistent and only on graph import!
                        int objectCnt = 0;
                        try {
                            String jsonStr = Helper.isToString(new FileInputStream(new File("./Resources/cameras.geojson")));
                            JSONObject json = new JSONObject(jsonStr);
                            JSONArray entries = json.getJSONArray("features");

                            EdgeExplorer explorer = tmpHopper.getGraphHopperStorage().createEdgeExplorer();
                            for (; objectCnt < entries.length(); objectCnt++) {
                                JSONObject entry = entries.getJSONObject(objectCnt);
                                JSONObject geo = entry.getJSONObject("geometry");
                                JSONArray coords = geo.getJSONArray("coordinates");

                                double lat, lon;
                                String type = geo.getString("type");

                                if (type.equals("Point")) {
                                    lat = coords.getDouble(1);
                                    lon = coords.getDouble(0);

                                } else if (type.equals("Polygon")) {
                                    // pick first point of polygon
                                    coords = coords.getJSONArray(0).getJSONArray(0);
                                    lat = coords.getDouble(1);
                                    lon = coords.getDouble(0);

                                } else {
                                    logger.warn(objectCnt + " unsupported geometry: " + type);
                                    continue;
                                }

                                QueryResult qr = index.findClosest(lat, lon, EdgeFilter.ALL_EDGES);
                                // include all surrounding edges too

                                EdgeIterator iter = explorer.setBaseNode(qr.getClosestNode());
                                while (iter.next()) {
                                    avoidEdgeIds.add(iter.getEdge());
                                }
                            }
                        } catch (Exception ex) {
                            logger.error("Error for " + objectCnt + " while json import " + ex.getMessage(), ex);
                        }
                        return tmpHopper;
                    }
                });
                install(new GHServletModule(args));

                bind(GuiceFilter.class);
            }
        };
    }

}
