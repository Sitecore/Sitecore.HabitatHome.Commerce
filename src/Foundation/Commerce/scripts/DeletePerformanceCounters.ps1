
$countersVersion = "1.0.2"
$commandCountersName = "SitecoreCommerceCommands-$countersVersion"
$metricsCountersName = "SitecoreCommerceMetrics-$countersVersion"
$listCountersName = "SitecoreCommerceLists-$countersVersion"
$counterCollectionName = "SitecoreCommerceCounters-$countersVersion"
[array]$allCounters = $commandCountersName,$metricsCountersName,$listCountersName,$counterCollectionName

Write-Host "Attempting to delete existing Sitecore Commmerce Engine performance counters"

# delete all counters
foreach($counter in $allCounters)
{
	$categoryExists = [System.Diagnostics.PerformanceCounterCategory]::Exists($counter)
	If ($categoryExists)
	{
		Write-Host "Deleting performance counters $counter" -ForegroundColor Green
		[System.Diagnostics.PerformanceCounterCategory]::Delete($counter); 
	}
	Else
	{
		Write-Warning "$counter does not exist, no need to delete"
	}
}



