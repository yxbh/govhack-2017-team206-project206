$url = "https://www.data.act.gov.au/resource/up3x-9a8h"
$apptoken = "sKOWMPx5jiwrYUQYHMgRYNlXV"

# Set header to accept JSON
$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
$headers.Add("Accept","application/json")
$headers.Add("X-App-Token",$apptoken)



$results = Invoke-RestMethod -Uri $url -Method get -Headers $headers