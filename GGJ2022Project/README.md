# Project Best Practices
## Coding Tenets
### DO Namespace your code with `namespace GGJ`
This will help us find each other's code in our IDE's and will make refactoring later easier. Use sub-namespaces as appropriate.
### DO Check out a new branch to work on a new feature
Name your branch with a prefix unique to you, and a small phrase describing the feature, e.g. `devin/pickup-system`
### DO Create a new Unity scene to test your feature
Create a directory for yourself in Scene/Tests and put your test scenes there. Only include the bare minimum objects and components in your scene to validate what you're working on.
### DO NOT Modify shared resources unless you are working on integration.
... and check with people first before merging changes back in.
### DO Squash merge back to main 
Merging your whole side branch history will make it hard to revert out breaking commits. You can also use an interactive rebase (`git fetch && rebase -i origin/main`) to ensure your side branch is up to date and will merge cleanly.
### DO Respect the separation between Runtime and Editor code
If it says `Using UnityEditor` at the top, it should be in an Editor folder. If you absolutely need to do an Editor thing in Runtime code, consider whether you can add a static utility function inside an Editor script instead.
### DO Check for permissively licensed Open Source or free Asset Store projects before starting a new system
Always double-check that you aren't about to write a bunch of code that already exists in a useable format. Also, be wary of free Script assets... most of the time they're more of a pain to integrate than they're worth.
### DO Always ask for help when you need it
The point of the Game Jam is to Learn New Things! Check the Resources channel for previously shared resources or ask for help/feedback in the help-and-feedback channel!
