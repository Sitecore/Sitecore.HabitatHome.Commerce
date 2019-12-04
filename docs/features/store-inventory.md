# Feature: Store Inventory

HabitatHome Commerce has a physical store feature. Stores can have an inventory for sellable items. Orders can also be shipped to stores.

By default, an Edmonton and an Ottawa stores are installed. They have inventory for most of the sellable items.

## How to Create Store Inventory Sets using JSON

1. Open the `C:\projects\Sitecore.HabitatHome.Commerce\docs\sample-data\store-inventory\InventoryStores.json` file in a text editor. It contains all the example stores.
2. Choose up to 2 stores you want to create. Delete the other ones.
3. If you want inventory only for a defined list of sellable items, edit the `ProductsToAssociate` array with the list you want.
4. On the other hand, if you want inventory for all the sellable items of a catalog:
   1. Specify the catalog name in the `Catalog` property.
      1. E.g.: `"Catalog":  "Habitat_Master",`
   2. Remove all items from the `ProductsToAssociate` array.
      1. E.g.: `"ProductsToAssociate": [],`
5. Open the InventoryUpload page: [https://habitathome.dev.local/utilities/inventoryupload.aspx](https://habitathome.dev.local/utilities/inventoryupload.aspx)
   1. Note: This page is coming from the **NearestStore** Helix feature website module of this repository. It works with the `CommandsController` from the engine project of the same feature module.
6. Copy the stores, catalog and productsToAssociate JSON and paste it in the InventoryUpload page input.
7. Click on the **Upload** button.
8. Wait for import to complete. This can take several minutes for inventory of the whole catalog. A green and red message will be displayed above the button when the import is complete or has failed.
9. Repeat the process with other stores as needed.

## When Upgrading Sitecore Experience Commerce

When upgrading, if the current inventory ZIP file is not in the right format and does not import the stores inventory:

1. Copy the out of the box Sitecore Experience Commerce inventory set (`Sitecore.Commerce.Engine.OnPrem.Azure.*.scwdp.zip\Content\Website\wwwroot\data\Catalogs\Habitat_Inventory.zip`) into your deployed commerce engine (`\wwwroot\data\Catalogs`).
2. Bootstrap, Cleanup, and Initialize the engine to get this inventory.
3. Follow the [How to Create Store Inventory Sets using JSON](#How to Create Store Inventory Sets using JSON) procedure to create the Edmonton and Ottawa stores inventory sets.
4. Using Postman, run the "Export Inventory Sets" request.
5. Save the response to a file in the repository (`[Your HabitatHome.Commerce clone path]\src\Project\HabitatHome\engine\wwwroot\data\Catalogs\Habitat_Inventory.zip`) and override the existing file.
6. Commit the new file.
