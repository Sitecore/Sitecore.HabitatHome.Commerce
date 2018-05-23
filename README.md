# Introduction 
HabitatHome Commerce Demo and the tools and processes in it is a Sitecore&reg; solution example built using Sitecore Experience Accelerator&trade; (SXA) on Sitecore Experience Platform&trade; (XP) and Sitecore Experience Commerce&trade; (XC) following the Helix architecture principles.

# Important Notice

### License
Please read the LICENSE carefully prior to using the code in this repository
 
### Support

The code, samples and/or solutions provided in this repository are ***unsupported by Sitecore PSS***. Support is provided on a best-effort basis via GitHub issues or [Slack #demo-sites](https://sitecorechat.slack.com/messages/habitathome/) (see end of README for additional information).

It is assumed that you already have a working instance of Sitecore XP **and** Sitecore XC  and all prerequisites prior to installing the demo. Support for **product installation** issues should be directed to relevant Community channels or through regular Sitecore support channels. 

### Warranty

The code, samples and/or solutions provided in this repository are for example purposes only and **without warranty (expressed or implied)**. The code has not been extensively tested and is not guaranteed to be bug free.  

# Getting Started

## Prerequisites

### Sitecore Version

Prior to attempting the demo installation, ensure you have a working **Sitecore XC 9.0.1** instance. Detailed installation instructions can be found at [doc.sitecore.com](http://commercesdn.sitecore.net/SitecoreXC_9.0/Installation-Guide/Sitecore-XC-9.0_Installation_Guide(On-Prem).pdf).

You do not need to install the Storefront Theme

**Clone this repository**

## Custom Install - before you start

If you do **not want to use the default settings**, you need to adjust the appropriate values in the following files:

`/gulp-config.js` 
`/publishsettings.targets` 
`src\Project\HabitatHome\code\App_Config\Include\Project\z.HabitatHome.Commerce.WebSite.DevSettings.config`

## Installation
**All installation instructions assume using PowerShell 5.1 in administrative mode.**
### 1 Clone the Repository
Clone the Sitecore.HabitatHome.Commerce repository locally - default settings assume **`C:\Projects\Sitecore.HabitatHome.Commerce`**. 

`git clone https://github.com/Sitecore/Sitecore.HabitatHome.Commerce.git` or 
`git clone git@github.com:Sitecore/Sitecore.HabitatHome.Commerce.git`

  
### 2 Deploy Solution
From the root of the solution

`npm install`
`node_modules\.bin\gulp initial`
> gulp **initial** only needs to be executed successfully during the initial deployment. Subsequent deployments can be made by running the default gulp task (gulp with no parameters). 
### 3 Deploy Engine

The next step will deploy Habitat Home's custom Commerce Engine with its relevant plugin and load the catalog, inventory and promotions.

The script is provided as an example and should be reviewed to understand its behavior prior to its execution. In summary, the script:

- Compiles and publishes the engine to a temporary location (default .\publishTemp)
- Makes changes to the configuration files to correctly set the certificate thumbprint, hostnames, etc)
- Stops IIS
- Creates a backup of the engine folders
- Copies the published and modified engine files to the webroot
- Starts IIS 
- Bootstraps the environment
- Cleans the environment (** erases all Commerce-related *data*)
- Initializes Environment (imports demo catalog, promotions, inventory, etc)


Assuming you have the default installation settings:

    deploy-commerce-engine.ps1 -Boostrap -Initialize

> if you have made any changes to your settings, review the deploy-engine-commerce.ps1 script and override / modify the parameters as required.


# Contribute or Issues
Please post any issues on Slack Community [#habitathome](https://sitecorechat.slack.com/messages/habitathome/) channel or create an issue on [GitHub](https://github.com/Sitecore/Sitecore.HabitatHome.Commerce/issues). Contributions are always welcome!