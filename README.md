## Blob Share Sample

The Blob Share sample Web application is a simple file sharing application that demonstrates the storage services of the Windows Azure Platform, together with the authentication and authorization capabilities of Access Control Service (ACS).
The application allows an administrator user to upload files and share them with other users by sending invitations. By leveraging the claims-based service of ACS, invited users will be able to authenticate using the social identity providers and get access to the shared files.

![Blob share screen](BlobShare/blob/master/images/blob-share-screen.png?raw=true)

### Prerequisites
* [Visual Studio 2012](http://www.microsoft.com/visualstudio/en-us/products) with SQL Express
* [ASP.NET MVC 4](http://www.asp.net/mvc/mvc4)
* [Windows Identity Foundation](http://www.microsoft.com/download/en/details.aspx?id=17331)
* [Windows Azure Libraries & SDK for .NET 1.7 - June 2012](http://www.windowsazure.com/en-us/develop/downloads/)
* [IIS 7.5 Express](http://www.microsoft.com/web/gallery/install.aspx?appid=IISExpress)

### Running the Sample Locally

##### Running the Sample Setup

The application requires you to specify several configuration settings before it can be used, including settings for the application's database server, an AppFabric Access Control Service namespace that the application uses for authentication, an SMTP server for invitations and notification emails, etc. All these configuration settings are entered in a **Configuration.xml** file in the **code** folder. To make it easy to run the sample out-of-the-box some values are pre-configured. 

> **Note:** When deploying the sample to Windows Azure, the pre-configured values need to be replaced. See the **Running the Sample in Windows Azure** section for instructions on how to do this.

1. Open the **Configuration.xml** file from the root folder of this sample.
2. The application sends invitations and can notify users whenever a resource is updated. To enable this functionality, you need to configure an SMTP server that the application can use for this purpose. For simplicity, you can use the Windows Live SMTP services. To do this, go to the **SMTP** tag, leave the default values and set the `username` and `password` values with Windows Live credentials.
3. Now run **Setup.cmd** from the **Setup** folder of the sample.
4. The script prompts you to confirm that you have entered the necessary settings into the Configuration.xml file. If you have done so, press **Y** to proceed with the installation
5. If you proceed with the installation, the script validates the information that you have entered and then executes all the required steps. Wait until the process is complete, which typically takes less than one minute.

	> **Note:** The script verifies whether the configured database is already present in the server and will prompt you for confirmation before dropping and recreating it. If you have executed the application previously, you may answer 'N' if you do not want to overwrite an existing database.

6. Once the installation is complete, you are ready to run the sample application with Visual Studio.

##### Running the Sample in Visual Studio

1. Start Windows Azure Storage Emulator.
2. Start Visual Studio 2010 and open the **BlobShare.sln** solution from the **code** folder of this sample.
4. Compile the solution. The NuGet packages dependencies will be automatically downloaded and installed.
5. Ensure that the **BlobShare.Web** cloud project is selected as the start-up project (shown in bold in Solution Explorer) and press F5 to run the sample.
6. When the application starts, you are prompted to log in. Note that the account used for the first time will be used as the site's administrator. Choose the identity provider of your preference and complete its sign in procedure.
7. Once logged in, your account needs to be verified. Enter your email address and the Bootstrap Administrator Secret value which by default is configured the Configuration.xml file to 123456 and then click Update.
8. Try inviting users and uploading new blobs.

### Running the Sample in Windows Azure
If you ran the application in the compute emulator previously, you will have already configured certain settings, for example the SMTP server configuration, that are common to both environments. Nevertheless, the following description does not assume any previous configuration and you should be able to prepare the application for deployment by following these steps. 

##### Creating an Access Control Service Namespace
The application takes advantage of the Access Control Service to enable authentication using one of several configured identity providers, for instance Windows Live ID, Yahoo!, Google, and Facebook.   
In this task, you configure an Access Control Service namespace that the application uses to enable this support.  

1.  Start by signing on to the Windows Azure Management Portal at <http://management.windowsazure.com/>.
2.  Hover mouse over the PREVIEW box at the top of screen, and then click on **Take me to the previous portal**. ![Go to previous portal](BlobShare/blob/master/images/go-to-previous-portal.png?raw=true)
3. Next, create a new **Service Namespace**. In the home page of the Windows Azure Management Portal, select **Service Bus, Access Control and Caching** in the lower half of the navigation pane.  
![Service Bus Menu](BlobShare/blob/master/images/service-bus-menu.png?raw=true)
4. Now, under the **Services** subarea in the upper half of the navigation pane, select **Access Control** and then click **New** on the ribbon.  
![New namespace](BlobShare/blob/master/images/new-namespace.png?raw=true)
5. In the **Create a new Service Namespace** dialog, choose a **Namespace** name and a **Country/Region** for the new namespace. Then, verify that the chosen namespace is available by clicking **Check Availability** and that **Access Control** is selected under **Available Services**. Make a note of the name you have chosen and then click **Create Namespace**.  
 ![New namespace dialog](BlobShare/blob/master/images/new-namespace-dialog.png?raw=true)  
6. Wait till the namespace is activated. Then, on the ribbon, click **Access Control Service** to browse to the ACS Management site.  
![ACS management site](BlobShare/blob/master/images/acs-management-site.png?raw=true)
7. In the ACS Management site, under **Administration** in the navigation pane, select **Management Service**. ![Management service](BlobShare/blob/master/images/management-service.png?raw=true)
8. In the **Management Service Accounts** section, select the **ManagementClient** service account. ![Management client](BlobShare/blob/master/images/management-client.png?raw=true)
9. In the **Edit Management Service Account** page, under **Credentials**, select **Symmetric Key**.   
![Symmetric key](BlobShare/blob/master/images/symmetric-key.png?raw=true)
10. In the **Edit Management Credential** page, make note of the value displayed for **Key**. You will need this value later, when you configure the application.  
 ![Copy key](BlobShare/blob/master/images/copy-key.png?raw=true)

##### Creating a Windows Azure Storage account
1.  Start by signing on to the Windows Azure Management Portal at <http://management.windowsazure.com/>.
2.  Click on the **NEW** link at the bottom-left corner of the screen. Then select **STORAGE**->**QUICK CREATE**. Enter an **URL** for your storage account. Pick **REGION/AFFINITY GROUP** of your choice (usually it should be the same region as where you ACS namespace is). Pick the subscription you want to use. Finally click **CREATE STORAGE ACCOUNT** to create the account.  
![New storage](BlobShare/blob/master/images/new-storage.png?raw=true)
3. Wait till the account is provisioned and appears on the dashboard list. Click on the name of the account to go to account details page. Then, click on the **MANAGE KEYS** link.  
![Manage keys](BlobShare/blob/master/images/manage-keys.png?raw=true)
4. In **Manage Access Keys** window, copy the **PRIMARY ACCESS KEY**. You'll need to enter it to the configuration file later.  
![access key detail](BlobShare/blob/master/images/access-key-detail.png?raw=true)

##### Creating a SQL Azure server
1.  Sign on to the Windows Azure Management Portal at <http://management.windowsazure.com/>.
2. Click on the **NEW** link at the bottom-left corner of the screen. Then, select **SQL DATABASE**->**CUSTOM CREATE**.
![custom create db](BlobShare/blob/master/images/custom-create-db.png?raw=true)
3. In **Specify database settings** dialog box, enter database **NAME**, and pick **New SQL Database Server** as **SERVER**. Then, click the Next arrow.  
![New server](BlobShare/blob/master/images/new-server.png?raw=true)
4. On next screen, create a SQL Database account. You'll need to enter the credential into **Configuration.xml** later on. Click on the check icon to complete this part of setup.   
![DB Server settings](BlobShare/blob/master/images/db-server-settings.png?raw=true)
5. Once the database has been created, you'll see an entry of the list on your dashboard. Take a note of the **SERVER** - this is the SQL Database server that you'll configure the application to use.  
![Server name](BlobShare/blob/master/images/server-name.png?raw=true)  
6. Click on the **SERVER** name to bring up server configuration page.
7. On server configuration page, click **CONFIG**. Then, in **allowed ip addresses** section, add current client IP to allowed IP address list. Click **SAVE** to save changes.  
![IP config](BlobShare/blob/master/images/ip-config.png?raw=true)
8. Open a command-line prompt. Go to **setup\scripts\database** folder.
9. Use command: 
                                                                                                   
		SqlAzure.Setup.cmd {SQL Database server}.database.windows.net {Database name} {SQL user} {SQL password} 
to create required tables. For example: 
                                                                                                   
		SqlAzure.Setup.cmd ixn1nx75ra.database.windows.net BlobShareDataStore adminuser password

##### Editing the Configuration.xml file to Provide the Configuration Settings
1. Open the **Configuration.xml** file from the root folder of this sample.
2. Near the top of the file, locate the **UseLocalComputeEmulator** setting and ensure that its value is set to _false_ to indicate to the setup script that you will deploy the application to Windows Azure. 
3. Now, locate the **BootstrapAdministratorSecret** entry and make a note of its value. You will be prompted for the administrator secret value the first time you sign in to the application to verify your identity. You can change the default value, if you wish to do so. 
4. The application stores its information in a relational database. To configure the database server used by the application, locate the **Database** section in the configuration file and enter the settings required to access the server that will host the application’s database. 
The table below summarizes each setting in this section.  
   
      **Setting** | **Description** | **Quick Configuraiton** 
  --------|--------|--------
  ServerName|Database server name. Specify an existing Windows Azure SQL Database server or leave blank to create a new server.|*(leave blank)*
  DatabaseName|Name of the application database. Default value is BlobShareDataStore.|*BlobShareDataStore*
  Username|User name of the database server login.|*Adminuser*
  Password|Password of the database login.|*(your-password)*
  Location|Location of a new SQL Database. This setting is used only when deploying to Windows Azure and creating a new SQL Database. Specify one of the available locations: North Central US, South Central US, North Europe, West Europe, East Asia, Southeast Asia, East US, or West US. Leave blank when using an existing server. Use the same location for the Web Site, the storage account, and the SQL Database.|*(your-location)* 
> Note: 
The **Quick Configuration** column in this table shows the values that you need to enter when using the automated scripts to create a new SQL Azure server in the chosen location and with the specified credentials.   

5. Authentication in the application is handled by the Access Control Service. To configure it, find the **AccessControlService** section and enter the settings for the service namespace that you created previously. Additionally, you can enable one or more identity providers that the application can use by setting the value of the corresponding **UseXXXProvider** element to _true_. By default, all the available providers except Facebook are enabled.   

    > Note: You need to update the **RelyingPartyRealm** when deploying the application to Windows Azure to use the URL of the Web Site where the application will be deployed.
The table below describes each setting in this section. 

    **Setting**|**Description**|**Quick Configuration**  
  --------|--------|--------
  Namespace|Name of the ACS namespace created for the application. Use the namespace only, not the full URL (e.g. blobshare).|*(your-namespace)*
  ManagementKey|Key for the management service account of the ACS namespace.|*(your-management-key)*
RelyingPartyRealm|Relying party realm. Identifies the relying party to the identity provider. When deploying to Windows Azure Web Sites, use the HTTP endpoint of your Web Site.|*https://{your-web-site}.azurewebsites.net/*
UseWindowsLiveIdentityProvider|Set to *true* to enable Windows Live ID as an identity provider.|*True*
UseYahooIdentityProvider|Set to *true* to enable Yahoo! as an identity provider.|*True*
UseGoogleIdentityProvider|Set to *true* to enable Google as an identity provider.|*True*
UseFacebookIdentityProvider|Set to *true* to enable Facebook as an identity provider. If Facebook is enabled as an identity provider, specify the Facebook application name, application ID and secret (see  http://msdn.microsoft.com/en-us/library/gg185919.aspx).|*False*
FacebookApplicationName|Facebook application name. Required if Facebook is enabled as an identity provider.|*(leave blank)*
FacebookApplicationId|Facebook application ID. Required if Facebook is enabled as an identity provider.|*(leave blank)*
FacebookSecret|Facebook application secret. Required if Facebook is enabled as an identity provider.|*(leave blank)*
> **Note:**
The **Quick Configuration** column in this table shows the values that you need to enter when deploying the application to Windows Azure, with Windows Live ID, Yahoo!, and Google enabled as identity providers. The values for the ACS namespace and management key are your own, which you obtained previously. For the relying party realm, you need to specify the URL of the hosted service where you plan to deploy the application (e.g _https://blobshare.azurewebsites.net/_). 
For more information on, see **Creating an Access Control Service Namespace** section.  

    > **Note:**
To use Facebook as an identity provider, you need to provide a Facebook application name, Id and secret. For detailed instructions on how to do this, see this article: <http://msdn.microsoft.com/en-us/library/gg185919.aspx>**.**

6. The application sends invitations and can notify users whenever a resource is updated. To enable this functionality, you need to configure an SMTP server that the application can use for this purpose. To do this, locate the **SMTP** section in the configuration file and enter the settings for the SMTP server of your provider.   
The table below summarizes each setting in this section. 

    **Setting**|**Description**|**Quick Configuration**
  --------|--------|--------
  Host|Name of the outgoing mail (SMTP) server (e.g. smtp.live.com).|*smtp.live.com*
  Port|SMTP server port number.|*25*
  User|Logon user name for the SMTP server.|*(your-username)*
  Password|Logon password for the SMTP server.|*(your-password)*
  SenderName|Name of the sender in invitation and notification emails.|*Blob Share*
  SenderAddress|Address of the sender in invitation and notification emails.|*(your-address)*

    > **Note:** The **Quick Configuration** column in this table shows the values that you need to enter when using Windows Live as your email service. 
If you choose an SMTP provider different from Windows Live, you need to update the host address and port to match the one used by your provider.
7. The application stores uploaded content in a container in blob storage. To configure the storage account, locate the **WindowsAzureStorage** section in the configuration file and enter the settings for the storage account to use. Note that if the specified account does not exist, it will be created by the deployment script.   

    > **Note:** 
Storage account names are essentially DNS names and thus shared with other subscriptions in the Windows Azure environment so choose a name that is unlikely to collide with other storage account names in existence (e.g. do **not** use _blobshare_, but prefer _blobshare<youruniquevalue>_ instead).
The table below summarizes each setting in this section. 

    **Setting**|**Description**|**Quick Configuration**
  --------|--------|--------
  StorageAccountName|Name of the storage account to use. When deploying to Windows Azure, if the specified storage does not exist, it will be created.|*(your-account-name)*
  StorageAccountKey|Access key of the storage account. Leave blank when creating a new storage account.|*(your-account-key)*
  StorageAccountLocation|Location of a new storage account. Specify one of the available locations: North Central US, South Central US, North Europe, West Europe, East Asia, Southeast Asia, East US, or West US. Leave blank when using an existing account. Use the same location for the Web Site, the storage account, and the SQL Database server.|*(your-account-location)*
StorageAccountLabel|Label for a new storage account. Leave blank when using an existing account.|*(your-account-label)*

##### Create Windows Azure Web Site
1. Browse to the Windows Azure portal at <https://manage.windowsazure.com>.
2. Click on the New button in the left corner and select **WEB SITE** -> **QUICK CREATE**. Enter a URL for the web site. Select the closest data center in the Region drop-down list. Click **CREATE WEB SITE** to create the web site.
![quick create web site](BlobShare/blob/master/images/quick-create-web-site.png?raw=true)
3. Back in the dashboard, click on the web site name in web sites list to bring up details page.
![web site in list](BlobShare/blob/master/images/web-site-in-list.png?raw=true)
4. In the **Quick Glance** navigation bar on the right side of the Dashboard, click the link labelled **Download Publish Profile**.   
This will download an XML file containing the Web Deploy deployment details. This file will not only enable publication of your web site, but also contains the connection information specific to the database you created with the site. This file, when used with Web Deploy from within Visual Studio 2012, is all you need to deploy your web site _and_ database to Windows Azure. 
![Download profile](BlobShare/blob/master/images/download-profile.png?raw=true)
5. Save the publish profile settings file to your local computer in a convenient location, as you'll be using it in the next step from within Visual Studio 2012.   
![save xml](BlobShare/blob/master/images/save-xml.png?raw=true)

##### Configure the Relying Party and Identity Providers
1.  Start by signing on to the Windows Azure Management Portal at <http://management.windowsazure.com/>.
2.  Hover mouse over the PREVIEW box at the top of screen, and then click on **Take me to the previous portal**. ![Go to previous portal](BlobShare/blob/master/images/go-to-previous-portal.png?raw=true)
3. Next, create a new **Service Namespace**. In the home page of the Windows Azure Management Portal, select **Service Bus, Access Control and Caching** in the lower half of the navigation pane.  
![Service Bus Menu](BlobShare/blob/master/images/service-bus-menu.png?raw=true)
4. Now, under the **Services** subarea in the upper half of the navigation pane, select **Access Control** and then click **New** on the ribbon.  
![New namespace](BlobShare/blob/master/images/new-namespace.png?raw=true)
5. Select the ACS namespace you created previously, then click **Access Control Service** to browse to the ACS Management site.  
![ACS management site](BlobShare/blob/master/images/acs-management-site.png?raw=true)
6. In the ACS Management site, under **Trust relationships** in the navigation pane, select **Relying party applications**. And then, click on the **Add** link.  
![manage rp](BlobShare/blob/master/images/manage-rp.png?raw=true)
7. Enter Relying party details as the following. Then click **Save**.

    **Field**|**Value**	
--------|--------
**Name**|a display name of your choice, for example “Blob Share”	
**Mode**|Enter settings manually	
**Realm**|http://{your website name}.azurewebsites.net/	
**Return URL**|http://{your website name}.azurewebsites.net/	
**Error URL (optional)**|(leave empty)	
**Token format**|SAML 2.0	
**Token encryption key**|None	
**Token lifetime (secs)**|600	
**Identity providers**|(check all identity providers)	
**Rule Group**|check Create new rule group	
**Token signing**|Use service namespace certificate (standard)	
8. Back in the management site, click **Rule group** link:  
![rule group](BlobShare/blob/master/images/rule-group.png?raw=true)
9. Click **Default Rule Group for** ***{your web site name}***  
![web site name](BlobShare/blob/master/images/web-site-name.png?raw=true)
10. Click the **Generate** link to generate default rules.  
![generate rules](BlobShare/blob/master/images/generate-rules.png?raw=true)
11. With all identity providers checked, click **Generate** button.  
![generate button](BlobShare/blob/master/images/generate-button.png?raw=true)
12. Click **Save** button.  
![save button 2](BlobShare/blob/master/images/save-button-2.png?raw=true)
13. Click **Identity providers** link in the left pane. Then, click **Add** link.  
![ip pane](BlobShare/blob/master/images/ip-pane.png?raw=true)
14. Select **Google** and then click **Next** button.  
![google](BlobShare/blob/master/images/google.png?raw=true)
15. Click **Save** button.  
![save google](BlobShare/blob/master/images/save-google.png?raw=true)
16. Repeat the same steps for **Yahoo!**.

##### Deploy Web Site
1. Open **code\BlobShare.sln** in Visual Studio.
2. Right click on **BlobShare.Web project in solution explorer and select **Publish...** from the context menu.  
![new context menu](BlobShare/blob/master/images/new-context-menu.png?raw=true)
3. In the **Publish Web** dialog, click the **Import** button. Navigate to the publish settings file you just downloaded, and select the file. If the dialog doesn't advance to the next step automatically once you select the file, click the **Next** button.    
![import xml](BlobShare/blob/master/images/import-xml.png?raw=true)
4. Click the **Validate Connection** button to verify the connection works and that the Web Deploy information is correct. Then, click the **Next** button to advance to the next step in the deployment process.  
![validate and publish](BlobShare/blob/master/images/validate-and-publish.png?raw=true)



