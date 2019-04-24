param($installPath, $toolsPath, $package, $project)
	
# find out where to put the files, we're going to create a deploy directory
# at the same level as the solution.
$rootDir = (Get-Item $installPath).parent.parent.fullname
$deployTarget = "$rootDir\"
# create our deploy support directory if it doesn't exist yet
$deploySource = join-path $installPath 'Deploy'

if (!(test-path $deployTarget))
{
	mkdir $deployTarget
}
	
# Datei kopieren
Copy-Item "$deploySource/*" $deployTarget -Recurse -Force
# get the active solution
$solution = Get-Interface $dte.Solution ([EnvDTE80.Solution2])
# create a deploy solution folder if it doesn't exist
$deployFolder = $solution.Projects | where-object { $_.ProjectName -eq "Solution Items" } | select -first 1
if(!$deployFolder) 
{
	$deployFolder = $solution.AddSolutionFolder("Solution Items")
}

# Datei in die Solution integrieren
$folderItems = Get-Interface $deployFolder.ProjectItems ([EnvDTE.ProjectItems])
$folderItems.AddFromFile( $deployTarget + "Settings.StyleCop")
