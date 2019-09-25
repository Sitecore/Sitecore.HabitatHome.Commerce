# Sitecore.HabitatHome.Commerce Development

## "Habitat Home Store" SXA Theme

### Editing the Theme

In order to be able to code review theme files changes, the files are under source control in the `/FrontEnd` folder.
Anytime a change needs to be done in the SXA theme:

1. Ensure the demo is deployed to Sitecore and working.
2. [Compare the source controlled front-end files with the ones in Sitecore](#comparing) to ensure source controlled files are up to date.
3. [Configure and start Creative Exchange Live](#live).
4. Do the required theme changes in the `[Your Commerce repo clone]\FrontEnd\-\media\Themes\Habitat SXA Sites\Habitat Home Store` folder. Changes will be synchronized to Sitecore items automatically (except changes in the gulp folder which must be uploaded in Sitecore manually).
5. When the update is done and you validated the site works correctly, ensure all modified front-end file has an associated modified serialized Sitecore item.
6. Commit front-end files and their associated serialized Sitecore items.
7. Create a pull request to review the changes.

### Updating Sitecore, SXA, or Commerce Version

When updating one of those dependencies, the site should be re-exported using Creative Exchange. The source controlled version should be updated.

1. [Compare the source controlled front-end files with the ones in Sitecore](#comparing) to ensure source controlled files are up to date.
2. [Configure and start Creative Exchange Live](#live).
3. Compare the out of the box Cart component (`[Your Sitecore webroot]\App_Data\packages\CreativeExchange\FileStorage\Habitat Home\-\media\Base Themes\Commerce Components Theme\Scripts\Cart`) files with our customized version (`[Your Commerce repo clone]\FrontEnd\-\media\Base Themes\Commerce Components Theme\Scripts\Cart`) and merge any differences in ours.
4. Validate the updated theme is still working correctly.
5. Fix any issues using Creative Exchange Live.
6. Ensure all modified front-end file has an associated modified serialized Sitecore item.
7. Commit front-end files and their associated serialized Sitecore items.
8. Create a pull request to review the changes.

<a id="comparing" name="comparing"></a>

### Comparing Source Controlled Front-End Files with the ones in Sitecore

1. From the Experience Editor, using SXA Creative Exchange, export the site to the file system. It will create the `[Your Sitecore webroot]\App_Data\packages\CreativeExchange\FileStorage\Habitat Home\-` folder.
2. Compare the `[Your Sitecore webroot]\App_Data\packages\CreativeExchange\FileStorage\Habitat Home\-` folder content with the `[Your Commerce repo clone]\FrontEnd\-` source controlled folder. They should be identical except for:
   1. The `-\media\Experience Explorer` folder. We do not source control this one.
   2. The `-\media\Images` folder. We do not source control this one.
   3. The `-\media\Project` folder. We do not source control this one.
   4. In our source controlled files, keep `-\media\Themes\Habitat SXA Sites\Habitat Home Store\gulpfile.babel.js`. This file should not be copied to the Creative Exchange export folder.
   5. In our source controlled `-\Themes\Habitat SXA Sites\Habitat Home Store\gulp\util` folder files, keep the `"rejectUnauthorized": false` modification.
   6. In our source controlled `-\media\Themes\Habitat SXA Sites\Habitat Home Store\gulp\config.js` file, keep the custom `https://habitathome.dev.local` server, and blank login/password.
   7. In our source controlled `-\media\Themes\Habitat SXA Sites\Habitat Home Store\gulp\serverConfig.json` file, keep our custom `projectPath` and `themePath`.
   8. In `-\media\Themes`, the `Habitat Home Basic` and `Habitat Home v2` folders. We do not source control these as they are from the Platform repository.
3. If different files are found, investigate the files and their associated YML items by looking at their Git history to determine which one should be kept. Update the files until both folders are identical.
4. If files were modified during the comparison, import the site using SXA Creative Exchange from the Experience Editor.
5. Revert item changes that seems unrelated to your changes:
   1. Modified items under `/sitecore/Content` (`/src/Project/HabitatHome/serialization/Content` in our repository).
6. When the update is done and you validated the site works correctly, ensure all modified front-end file has an associated modified serialized Sitecore item.
7. Commit front-end files and their associated serialized Sitecore items.

<a id="live" name="live"></a>

### Starting Creative Exchange Live

1. If your login/password is not admin/b, update the login/password in `[Your Commerce repo clone]\FrontEnd\-\media\Themes\Habitat SXA Sites\Habitat Home Store\gulp\config.js`.
2. If you run the demo using a custom host name, update **serverOptions.server** in `[Your Commerce repo clone]\FrontEnd\-\media\Themes\Habitat SXA Sites\Habitat Home Store\gulp\config.js`.
3. Enable the `[Your Sitecore webroot]\App_Config\Include\z.Feature.Overrides\z.SPE.Sync.Enabler.Gulp.config.disabled` config file.
4. Open an elevated shell (cmd or PowerShell) in `[Your Commerce repo clone]\FrontEnd\-\media\Themes\Habitat SXA Sites\Habitat Home Store`
5. Install Gulp CLI and the project node modules.
   1. Run `npm install -g gulp-cli`
   2. Run `npm install` (This will also create the `gulpfile.babel.js` file which is git ignored)
6. To run the gulp tasks run `gulp`. This will watch the local folders and copy the files in the Sitecore media library.
