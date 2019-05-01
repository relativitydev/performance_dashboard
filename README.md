# performance_dashboard
Read Only:  This is a repository for the code for Performance Dashboard.  We are not currently accepting pull requests for this repo as we have no Project Champion for it.  If you are interested in becoming the Project Champion please let us know.  Feel free to fork the repository and make any modifications you or your team see fit as long as it abides by the license.

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
 
## Miscellaneous
 
Project is developed by Milyli Inc.
