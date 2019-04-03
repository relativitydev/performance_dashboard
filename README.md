# Performance Dashboard - Helps you monitor and analyze your Relativity environment.
 
## Overview
 
Relativity Performance Dashboard helps you monitor and analyze your Relativity environment in real time. Intuitive charts and reports provide you with greater insight into your environment to quickly assess potential areas of concern. Performance Dashboard is available for download through the Relativity Customer Portal.
 
## How to Build
 
 * Run ./build.ps1
	* Accepts Debug/Release params, as well as specifying a version number with -v
 
## How to Test
 
 * Run DevelopmentScripts/integration-UpdateConfiguration.ps1
	* This will create AppSettings.Override.config and RelativityConnection.Override.config files
 * Edit AppSettings.Override.config and RelativityConnection.Override.config files with info to point to your testing environment
 
## Build artifacts
 
 * After executing ./build.ps1, rap can be found in "Applications/Performance Dashboard.rap"
 * Builds are built automatically in TeamCity: https://teamcity.kcura.corp/project.html?projectId=PerformanceDashboard
	* Successful builds can be found here: file://bld-pkgs.kcura.corp/Packages/PerformanceDashboard/
 
## Maintainers
 
Performance Dashboard Team (Team Full Tilt) <PDB@milyli.com>
Product Manager
 * Marko Iwanik <marko@milyli.com> <marko.iwanik@relativity.com>
 * Joseph Low <joseph@milyli.com> <joseph.low@relativity.com>
Developers
 * David Grupp <david@milyli.com> <david.grupp@relativity.com>
 * Tony Zahnle <tony@milyli.com> <tony.zahnle@relativity.com>
Quality Assurance
 * Cameron Sery <cameron@milyli.com> <cameron.sery@relativity.com>
 * Sofiia Tsapchuk <sofiia@milyli.com> <sofia.tsapchuk@relativity.com>
 
## Miscellaneous
 
Project is developed by Milyli Inc.