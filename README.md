# Introduction 
The instructions in this README will guide you through the installation process of the Habitat Home Commerce Demo. 

There is a dependency on **Sitecore.Habitat** and **Sitecore.Habitat.Home** which will be detailed below.


# Getting Started
This guide assumes you've cloned and deployed both Sitecore.Habitat and Sitecore.Habitat.Home. See the README.md file in the Sitecore.Habitat repository.

**Assumption**: the configuration file used to deploy Sitecore.Habitat still exists. It is required during the installation process.

**Clone this repository**

## Custom Install - before you start

If you do **not want to use the default settings**, you need to adjust the appropriate values in the following files in the project:

Create a user version of the following files

**Sitecore.Habitat.Commerce**
`/gulp-config.user.js` 
`/publishsettings.user.targets` 
`/TDSGlobal.config.user` (only if using TDS)
`src\Project\Habitat.Commerce.Website\code\App_Config\Include\Project\z.Habitat.Commerce.WebSite.DevSettings.user.config`

## Installation
**All installation instructions assume using PowerShell 5.1 in administrative mode.**
### 1 Clone the Repository
Clone the Sitecore.Habitat.Commerce repository locally - default settings assume **`C:\Projects\Sitecore.Habitat.Commerce`**. 
`git clone https://sitecoredst.visualstudio.com/Demo/_git/Sitecore.Habitat.Commerce` or 
`git clone ssh://sitecoredst@vs-ssh.visualstudio.com:22/Demo/_ssh/Sitecore.Habitat.Commerce`

### 2 Prerequisites

*Most* prerequisites, if not already installed for Sitecore.Habitat are automatically downloaded and installed. 

### 3 Preparing Configuration file
Make a copy of `set-installation-overrides.ps1.example` and remove the .example extension

`cd install`
`Copy-Item set-installation-overrides.ps1.example set-installation-overrides.ps1`

Edit set-installation-overrides.ps1 to suit your installation needs.

### 4 Generate configuration file
Execute the following two commands to generate `configuration-xc0.ps1`
`.\set-installation-defaults.ps1`
`.\set-installation-overrides.ps1`

### 5 Install Commerce
Start the installation process
`install-xc0.ps1`

### 6 Deploy Solution
Navigate back to the root of the repository (*c:\projects\sitecore.habitat.commerce*)
`npm install`
`node_modules\.bin\gulp`

## Issues / Need Help
Please post on Teams - Sitecore Demo (Help and Discussions channel) if you encounter any issues.