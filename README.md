# Music Fall 2016

##Starter code for ASP.NET Core MVC assignment

After cloning this repo, it is necessary to build the database. Use the Package Manager Console for this. Since the project already has the initial migration included it is only necessary to update the database.

    PM> update-database

(This commands might generate errors: `Project "Default" is not found`)

This might be due to some ongoing tooling changes. If you get this, do the following

Change to the directorory containing the project.json file (modify the directory name if you called it something else):

    PM> cd src/MusicFall2016

Then use this command:

    PM> dotnet ef database update

To add your own migration, use

    PM> dotnet ef migration add myupdate


Note that `myupdate` is a name that identifies the migration. Pick any name that is self-descriptive of what the migration is intended to do.
