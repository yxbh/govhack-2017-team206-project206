$url = "https://www.data.act.gov.au/resource/up3x-9a8h"
$locs = "https://www.data.act.gov.au/resource/h534-v2x9"
$apptoken = "sKOWMPx5jiwrYUQYHMgRYNlXV"

# Set header to accept JSON
$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
$headers.Add("Accept","application/json")
$headers.Add("X-App-Token",$apptoken)



$fines = Invoke-RestMethod -Uri $url -Method get -Headers $headers

$locations = Invoke-RestMethod -Uri $locs -Method get -Headers $headers 

foreach ($location in $locations) {
   
    Invoke-RestMethod -Uri $url -Method get -Headers $headers -Body {location_code : $(location.location_code) }
    
}