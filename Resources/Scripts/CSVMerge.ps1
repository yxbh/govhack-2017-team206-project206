$newCSV = @{}




$fines = import-csv -Path "~/Documents/Traffic_camera_offences_and_fines.csv" 

$locations = import-csv -Path "~/Documents/Traffic_speed_camera_locations.csv" | Group-Object -Property location_code -AsHashTable

$newtable = $locations | select location_code, Latitude, Longitude, "location DESCRIPTION", @{n="locationcode";e={$locations.($_.location_code).item(0).location_code}}




$firstinfo = Import-Csv c:\ee\first.csv | Group-Object -Property employee_number -AsHashTable
Import-Csv c:\ee\second.csv | 
	Select-Object name, ntaccountname, employeenumber, 
		@{n="title"; e={$firstinfo.($_.employeenumber).item(0).title}} | 
			Export-Csv c:\ee\new.csv -NoTypeInformation

$loop = foreach ($fine in $fines){


ForEach ($loc in $newtable){

if ($loc.location_code -contains $fine.location_code){

Write-Host $loc.location_code "`t "$fine.location_code

}
}

}




$starttime = (Get-Date)
Write-host "Script Started time:" $Starttime

$Hosts = Import-csv "~/Documents/Traffic_camera_offences_and_fines.csv" | Select -ExpandProperty Hostname

$BookA = ForEach ($Line in (Import-Csv c:\test\FileA.csv))
{ If ($Hosts -contains $Line.Hostname)
  { $Line | Select 'Hostname','COLA','COLB'
      }
   
}
$BookB = ForEach ($Line1 in (Import-Csv c:\test\FileB.csv))
{ If ($Hosts -contains $Line1.Hostname)
  { $Line1 | Select 'Hostname','COLC','COLD'}
}

$Result = $( foreach($data in $BookA)
{

$t = $BOOKB | Where-Object {$_.Hostname -eq $data.HostName }
$t1 = $data | select *


foreach ($p in Get-Member -InputObject $t -MemberType NoteProperty)
{

Add-Member -InputObject $t1 -MemberType NoteProperty -Name $p.Name -Value $t.$($p.Name) -Force 
$t.$($p.Name) = $t1.$($p.Name)
}

$t1

}
)

$Result |select 'HostName','COLA','COLB','COLC','COLD'| Export-Csv C:\test\Result.csv -notypeinformation | ft -autosize
$endtime = (Get-Date)
  "Elapsed Time: $(($endtime-$starttime).totalseconds) seconds"

