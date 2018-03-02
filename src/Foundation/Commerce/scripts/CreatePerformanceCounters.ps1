
$ccdTypeName = "System.Diagnostics.CounterCreationData"
$countersVersion = "1.0.2"
$perfCounterCategoryName = "SitecoreCommerceEngine-$countersVersion"
$perfCounterInformation = "Performance Counters for Sitecore Commerce Engine"
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

Write-Host "`nAttempting to create Sitecore Commmerce Engine performance counters"

# command counters
Write-Host "`nCreating $commandCountersName performance counters" -ForegroundColor Green
$CounterCommandCollection = New-Object System.Diagnostics.CounterCreationDataCollection
$CounterCommandCollection.Add( (New-Object $ccdTypeName "CommandsRun", "Number of times a Command has been run", NumberOfItems32) )
$CounterCommandCollection.Add( (New-Object $ccdTypeName "CommandRun", "Command Process Time (ms)", NumberOfItems32) )
$CounterCommandCollection.Add( (New-Object $ccdTypeName "CommandRunAverage", "Average of time (ms) for a Command to Process", AverageCount64) )
$CounterCommandCollection.Add( (New-Object $ccdTypeName "CommandRunAverageBase", "Average of time (ms) for a Command to Process Base", AverageBase) )
[System.Diagnostics.PerformanceCounterCategory]::Create($commandCountersName, $perfCounterInformation, [Diagnostics.PerformanceCounterCategoryType]::MultiInstance, $CounterCommandCollection) | out-null

# metrics counters
Write-Host "`nCreating $metricsCountersName performance counters" -ForegroundColor Green
$CounterMetricCollection = New-Object System.Diagnostics.CounterCreationDataCollection
$CounterMetricCollection.Add( (New-Object $ccdTypeName "MetricCount", "Count of Metrics", NumberOfItems32) )
$CounterMetricCollection.Add( (New-Object $ccdTypeName "MetricAverage", "Average of time (ms) for a Metric", AverageCount64) )
$CounterMetricCollection.Add( (New-Object $ccdTypeName "MetricAverageBase", "Average of time (ms) for a Metric Base", AverageBase) )
[System.Diagnostics.PerformanceCounterCategory]::Create($metricsCountersName, $perfCounterInformation, [Diagnostics.PerformanceCounterCategoryType]::MultiInstance, $CounterMetricCollection) | out-null

# list counters
Write-Host "`nCreating $listCountersName performance counters" -ForegroundColor Green
$ListCounterCollection = New-Object System.Diagnostics.CounterCreationDataCollection
$ListCounterCollection.Add( (New-Object $ccdTypeName "ListCount", "Count of Items in the CommerceList", NumberOfItems32) )
[System.Diagnostics.PerformanceCounterCategory]::Create($listCountersName, $perfCounterInformation, [Diagnostics.PerformanceCounterCategoryType]::MultiInstance, $ListCounterCollection) | out-null

# counter collection
Write-Host "`nCreating $counterCollectionName performance counters" -ForegroundColor Green
$CounterCollection = New-Object System.Diagnostics.CounterCreationDataCollection
$CounterCollection.Add( (New-Object $ccdTypeName "ListItemProcess", "Average of time (ms) for List Item to Process", AverageCount64) )
$CounterCollection.Add( (New-Object $ccdTypeName "ListItemProcessBase", "Average of time (ms) for a List Item to Process Base", AverageBase) )
[System.Diagnostics.PerformanceCounterCategory]::Create($counterCollectionName, $perfCounterInformation, [Diagnostics.PerformanceCounterCategoryType]::MultiInstance, $CounterCollection) | out-null



