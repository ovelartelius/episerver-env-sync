# Episerver CMS environment synchronizer
If you are moving database backups between environments and have specific hostnames that should be changed when run in another environment. Then you can use this NuGet package to solve this problem. You can also update ScheduledJobs if you need to enable/disable the jobs in different environments.  
  
The synchronizer can be run as a InitializationModule or as a ScheduleJob. It depends on what you think is fitting your environment and project.

## Installation
Grab the latest dll from //dlls/EnvironmentSynchronizer.[version] and reference that in your project.  
When we are able to publish this as a NuGet the isntallation will be a regular NuGet installation of the package "".

## Configuration
Example web.config
```xml
<configuration>
  <configSections>
    <section name="env.synchronizer" type="EnvironmentSynchronizer.SynchronizerSection" allowLocation="true" allowDefinition="Everywhere" />
  </configSections>
	<env.synchronizer runAsInitializationModule="true">
		<sitedefinitions>
			<sitedefinition Id="" Name="AlloyDemo" SiteUrl="http://localhost:58288/">
				<hosts>
					<host Name="*" UseSecureConnection="false" Language="" />
					<host Name="local.alloydemo.se" UseSecureConnection="false" Language="en" />
					<!--Type: Undefined|Edit|Primary|RedirectPermanent|RedirectTemporary-->
				</hosts>
			</sitedefinition>
		</sitedefinitions>
		<schedulejobs>
			<schedulejob Id="*" Name="*" IsEnabled="false" />
			<schedulejob Name="Episerver-notifieringar" IsEnabled="true" />
		</schedulejobs>
	</env.synchronizer>
```

### runAsInitializationModule
Tells the synchronizer that you want to run it as InitializationModule. All settings that if can update will be executed.

### sitedefinition
**Id** is the GUID that identify the site. If this is provided it will ignore the "Name" attribute.
**Name** is the name of the sitedefinition that will be updated. If **Id** is not specified it will match the existing SiteDefinition in the Episerver CMS against this name.
**SiteUrl*** is the SiteUrl that this site should have/use. If the existing SiteDefinition that are found with Id/Name in Episerver CMS already have a SiteUrl. The SiteDefinition in Episerver CMS will not be updated.

### hosts
You need to specify all the hosts that the site needs. When the synchronizer is updating a SiteDefinition it will expect that you have specified all hostnames. So of you in Episerver CMS has a extra host that is not specified in the web.config it will be removed.

### host
**Name** is the hostname. Example local.alloydemo.se  
**UseSecureConnection** specify if it is a http/https URL.  
**Type** is the type of the host. It is the enum EPiServer.Web.HostDefinitionType that are used by Episerver CMS. If the Type is not specified it will be set to `Undefined`.  
Options (EPiServer.Web.HostDefinitionType [Enum]):  
- **Undefined**
- **Edit**
- **Primary**
- **RedirectPermanent**
- **RedirectTemporary**
**Language** is the CultureInfo that is related to the hostname  

## scheduledjobs
You can specify 0 to many Scheduledjob that should be updated.

### schedulejob
**Id**: If Id is specified then the synchronizer will ignore the Name and find the scheduled job that match the Id.  
**Name** The name of the job that you want to update. You can use `*` as a wildcard. That means that it will go through all ScheduledJobs in Episerver CMS and enabled/disabled them. So you should have this as the first definition in the configuration.  
Example:  
```xml
  <schedulejobs>
    <schedulejob Id="*" Name="*" IsEnabled="false" />
    <schedulejob Name="Episerver-notification" IsEnabled="true" />
  </schedulejobs>
```  
In this example it first go through all ScheduledJobs and desable them. And then it will enable the job "Episerver-notification".
**IsEnabled** [bool] set if the job should be enabled/disabled. 
