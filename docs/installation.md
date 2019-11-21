# Installing Sitecore.HabitatHome.Commerce

## Prerequisites

### HabitatHome Platform Demo

Prior to attempting the HabitatHome Commerce demo installation, ensure you have a working Sitecore HabitatHome Platform demo installed on Sitecore 9.2.0.

If you have not yet done so, deploy the base HabitatHome.Platform solution. See the `README.md` in the [Sitecore.HabitatHome.Platform](https://github.com/sitecore/sitecore.habitathome.platform) repository.

You can run the `Quick-Deploy` Cake target from the root of the HabitatHome.Platform solution for a quicker deploy that excludes post deploy actions or Unicorn synchronization.

`.\build.ps1 -Target "Quick-Deploy"`

### Sitecore Commerce

Prior to attempting the demo installation, ensure you have a working **Sitecore XC 9.2.0** instance. Detailed installation instructions can be found at [doc.sitecore.com](http://commercesdn.sitecore.net/SitecoreXC_9.2/Sitecore_XC-9.2_Installation_Guide_for_On-Prem.pdf).

You do not need to install the Storefront Theme

*IMPORTANT: Publish site after installation*

### Clone of This Repository

Clone the Sitecore.HabitatHome.Commerce repository locally - default settings assume **`C:\Projects\Sitecore.HabitatHome.Commerce`**.

`git clone https://github.com/Sitecore/Sitecore.HabitatHome.Commerce.git` or
`git clone git@github.com:Sitecore/Sitecore.HabitatHome.Commerce.git`

## Installation

**All installation instructions assume using PowerShell 5.1 in administrative mode.**

### Custom Install - Before You Start (Optional)

If you do **not want to use the default settings**, you need to adjust the appropriate values in the `/cake-config.json` file.

Note: If you've already deployed the HabitatHome Platform demo, and you wish to run the HabitatHome Commerce demo in a new instance by customizing these settings,
you would need to also customize the settings in the HabitatHome Platform demo and deploy it to the new instance **before** deploying HabitatHome Commerce. See `README.md` in Sitecore.HabitatHome.Platform for custom settings.

### Publish Site Before Starting

Smart publish the entire Sitecore site before starting. It is required between Sitecore Experience Commerce installation and this demo.

### Deploy Solution

To deploy the **HabitatHome.Commerce** solution, from the root of the solution

`.\build.ps1 -Target Initial`

Note: Build target **`Initial`** only needs to be executed successfully during the initial deployment. Subsequent deployments can be made by running the **`Default`** Cake build target: `.\build.ps1` (without target specification).

### Set Custom Hostnames (optional)

If you set custom hostnames for this deployement, you need to set them in some Sitecore items after deployment.

Master database:
- `/sitecore/content/Habitat SXA Sites/Habitat Home/Settings/Site Grouping/Habitat Home`: Set the Sitecore website hostname in the "Host Name" field.

Core database:
- `/sitecore/client/Applications/Launchpad/PageSettings/Buttons/Commerce/BusinessTools`: Set the commerce business tools URL in  the "Link" field.

### Deploy Engine

The next step will deploy Habitat Home's custom Commerce Engine with its relevant plugin and load the catalog, inventory and promotions.

Notes:

- If you want to use your own engine suffix rather than `habitathome`, you need to update it in `deploy-commerce-engine.ps1`
- If you want to use your own databases rather than `habitathome_Global`, you need to update it in `\src\Project\HabitatHome\engine\wwwroot\bootstrap\Global.json`
- If you want to use your own databases rather than `habitathome_SharedEnvironments`, you need to update it in `\src\Project\HabitatHome\engine\wwwroot\data\Environments\Plugin.SQL.PolicySet-1.0.0.json`

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

If you have made any changes to your settings, review the `deploy-engine-commerce.ps1` script and override / modify the parameters as required.

`.\deploy-commerce-engine.ps1 -Bootstrap -Initialize`

### Rebuild Indexes

Rebuild:

- sitecore_master_index (For media selector dialog to display media)
- sitecore_web_index (For product listing pages to start displaying products)

### Publish Site

Smart publish the entire Sitecore site.
