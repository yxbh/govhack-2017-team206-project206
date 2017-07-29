# Avoid your next speeding fine 🚔🎥🏎️⏩️💸

Simply type in where you want to go and we'll give you directions with the least probability of getting a speeding ticket based on the amount of fines handed out for particular roads!

# License

??? something public

# Installation

 * git clone
 * get the area of your choice in OSM PBF (Open Street Map protobuff) format 
 * update the export.geojson if not Canberra (defaults to canberra traffic camera dataset)
 * `mvn clean install assembly:single`
 * `java -jar target/avoid-camera-route-Canberra-1.0-SNAPSHOT-jar-with-dependencies.jar osmreader.osm=Resources/canberra.osm.pbf config=config.properties`
 * change the weighting in the URL to 'avoidcamera' instead of fastest (or another type of weighting)
