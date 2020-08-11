# WombatGH #

This is the source code for the Wombat Grasshopper library. The most recent build can be found on [Food4Rhino](https://www.food4rhino.com/app/wombatgh)

## How to Use ##

This project is built using [Visual Studio](https://visualstudio.microsoft.com/), so you'll need to install that to get started. After that:
1. `git clone https://github.com/woodsbagot/WombatGH.git `
2. Open the solution (WombatGH/WombatGH.sln) in Visual Studio.
3. Build the solution (right click on Solution 'WombatGH' -> Build Solution)
4. The Grasshopper library (GHA) file is configured to build to `WombatGH/bin/<config>/WombatGH.gha`. You'll need to copy it to your Grasshopper components directory, or use GrasshopperDeveloperSettings command to tell Grasshopper to look in your /debug/ directory for the assembly.

## Find a bug? Want a new feature?
[Open an issue](https://github.com/woodsbagot/WombatGH/issues/new/choose) on the project's Github page. Please check previous issues to make sure you're not duplicating what's already there, and be as descriptive as possible! If you're up for it, you could even fix the bug yourself & become a contributor:

## Contributing ##

We're excited that you want to help improve WombatGH! This project uses a forking workflow for contributions. You can read about the basics [here](https://guides.github.com/activities/forking/). An overview:
1. Fork this repo to create your own copy of the repo.
2. Clone your fork so you can make local changes.
3. Commit your changes to your fork.
4. When you're ready, open a pull request. We'll review it, and if it's approved we'll merge your fork back into the master.

Reminder, you may want to keep your fork in sync with the master. This doesn't happen automatically! These two commands will pull the changes from the this repository and push them to your forked repository:
```
git pull upstream master
git push origin master
```

## Contributors ##

These tools were initially built by [Andrew Heumann](https://github.com/AndrewHeumann) and [Brian Ringley](https://github.com/bringley) on behalf of the [Woods Bagot](http://www.woodsbagot.com) Design Technology team. Previously maintained by [Matthew Swaidan](https://github.com/mswaidan). They are currently maintained by [Andrea Tassera](https://github.com/Dre-Tas).

A big thank you to all the Woods Bagot project staff whose requests and testing made these tools possible!
	
## License 
[GPL v3.0](https://github.com/woodsbagot/WombatGH/blob/master/LICENSE)
