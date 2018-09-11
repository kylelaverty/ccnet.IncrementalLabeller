# Incremental labeller plugin for CCNET

This labeller plugin builds the CCNet internal label from the last label combined with a specified increment.

As any other CCNet labeller, used in conjunction with the AssemblyInfo task in  [MsBuildTasks](https://github.com/loresoft/msbuildtasks) or with a manual script it allows setting the AssemblyVersion - AssemblyFileVersion attributes of an MSBuild project.

## Building the label

* Take the last label
* Take the Major, Minor and Build if configured and overwrite the last label
* If an increment is specified, increment the revision by that, otherwise increment the revision by 1
* If the Major/Minor/Build value has changed since the last label, and reset revision is set, update revision to 0
* If the last run is not success and increment of failure is false, reset revision to the last label value
* New label: [Major].[Minor].[Build].[Revision]

## Usage
```xml
	<labeller type="incrementalLabeller">
		<major>10</major>
		<minor>20</minor>
		<build>30</build>
		<revisionIncrement>10</revisionIncrement>
		<resetRevisionAfterVersionChange>true</resetRevisionAfterVersionChange>
		<incrementOnFailure>false</incrementOnFailure>
	</labeller>
```

* *major* is optional (defaults to previous value)
* *minor* is optional (defaults to previous value)
* *build* is optional  (defaults to previous value)
* *revisionIncrement* is optional (defaults to 1)
* *resetRevisionAfterVersionChange* is optional (defaults to false)
* *incrementOnFailure* is optional (defaults to false)

## How to make use of the label	

* In an MSBuild target you may access the value as $(CCNetLabel)
* In NAnt you may use $[CCNetLabel]
* To use the value inside the CCNet config blocks, you need to write it as a dynamic parameter, i.e. $[$CCNetLabel] . This is valid only for CCNet 1.5+

## Installation ##

* Before building, update the *ThoughtWorks.CruiseControl.Core.dll* and *ThoughtWorks.CruiseControl.Remote.dll* in the *lib* folder with the ones corresponding to your CCNET version (found in *server* subfolder of the CruiseControl.Net program files folder)

* Build the solution

* Copy the resulting *ccnet.IncrementalLabeller.plugin.dll* into the *server* subfolder.

NOTE: the assembly file name must follow the ccnet.*.plugin.dll pattern - in order to be loaded by CCNET