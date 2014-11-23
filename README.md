ssauthz
=======
A basic oAuth2.0 authorization server in ServiceStack + DNOA

This service is implemented using ServiceStack + DNOA, and offers a REST and Forms based API interface for managing clientApplications, UserAccounts and AccessTokens used for oAuth 2.0. access.

Update (24/11/2014)
------
The integration tests are not working at present because I havent had the time to fully finish the 'InMemoryStoreContext' class which acts as persistence store in the service. I still need to implment the 'Find()' method which takes a textual query string and run that on a generic entity. Any help would be appreciated to get that done.
Also, I still need to add an simple example service that uses oAuth2.0. access-tokens that are verified before any code is run. he attributes are already in this sample, I just need to demonstrate an example of an SS service using them. 


Some things you need to be aware of:
--------------------------------------

* This implmentation has been ripped out of a larger more complex product, so many of the patterns in the code shown here are a little more complex than it would be for a simple code snippet. Resharper also automatically reverted some of our own coding standards when we migrated to code and did a code cleanup - so I can't guarantee you will like how the code is formatted or laid out. 
* All shared code that existed outside the main web service project ('Services.AuthZ') has been condensed in the 'Common' project for this sample, so you might need to split some of that out into your own shared libraries.
* The great deal of this code was generated with custom developer tooling we made (because of its predictable nature - no one needs to hand type that kind of stuff!) that is what the *.gen.cs files are all about. You can rename them and remove the headers from them if you want.
* All projects in this solution share a common 'BuildConfiguration.targets' file (<Import> statement in *.csproj file) which basically provides unified build configuration settings and signs the assemblies with a single strong name (found in root of solution). You might do things very differently in your solution, and you would certainly use a different strong name file in your solution.
* This AuthZ server does no custom authentication other then verify username/password. You may extend your AuthZ server to do other things oAuth2.0'ish, like check with some user account settings to see if they still permit these client applications access or not. See DNOA docs for ideas.
* This solution used to use Azure Table Storage to store accounts, access-tokens etc, but we had to rip all that out and replace with a InMemoryStore for ease of getting this up as a sample. You will want to store your data in your data store. So replace the 'InMemoryStoreContext' class with your own, or do the storage layer your own way. 
* We have hardcoded the clientidentifiers+clientsecrets in a code file for ease of the sample ('Common/Clients.cs'). You would not do this in your production solution, store these secrets elsewhere!
* The AuthZ service has SwaggerUI installed to display developer docs for the API of the AuthZ server. It is nice and handy, but you can remove it if you want.
* The AuthZ service must be configured as SSL (HTTPS), otherwise oAuth2.0 is totally compromised and insecure! That makes development a little less easy, but you totally compromise security of oAuth2.0 if you turn it off, as you are sending secrets in the clear with oAuth2.0! You should not have to turn of SSL in development at all.
* This AuthZ implementation signs its access-tokens/refresh_tokens to prevent spoofing etc. You will need to install an signing certificate in your dev machine (and on production servers) for this solution to work. It loads the cert at runtime from your 'localmachine/my' store (see Common.Security.CryptoKeyProvider). You will find the self-signed cert this sample uses in the 'src\Certificates' folder. Install it on your dev machine. Its name is referenced in the config of the Services.AuthZ project.
* We have given you most of the Unit tests for these classes, and have also included integration tests that fire up the AuthZ service in IISExpress, and test it end-to-end through the REST API.


Some things you must do to get this going
-----------------------------------------
1. You must define your own: servicestack license key (in Common.Licensing.ServiceStackLicensekey) as this sample uses the supported version of ServiceStack. (4.*)
2. You will want to define your oAuth scopes (currently in Common.AccessScope.Profile)
3. You need to update the 'Services.Authz\web.config' to reflect your own configuration
4. You will need to change the clientidentifier+clientsecret in 'Common.Clients', and later store these secrets elsewhere in your deployment.
5. You need to install the 'src\Certificates\Signing.AuthZServer.pfx' into your localmachine/personal store, and copy it into the 'Trusted' folder. Use MMC.exe to do that. Later you will want o create your own signing cert for your solution.
6. You need to update the app.config in Services.IntTests to reflect your installation.