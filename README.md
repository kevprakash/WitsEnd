# WitsEnd

## Project Setup Instructions
1. Create a new 2D project with Unity Version 2020.1.10f1 (other versions might work but this is the one it was originally made on)
   - Make sure to take note of the project folder 
3. Close Unity once the editor opens up and everything is done loading
4. Go to the project folder and go into the Assets folder
5. Open gitbash (or whatever you use for commandline git) use the command "git clone https://github.com/kevprakash/WitsEnd.git ." without the quotes
6. Once it finishes downloading everything, open up the project in Unity again (it will probably take a while to start the first time)
8. Everything should be ready from there on out

## Updating with Changes
1. Save all the changes you want
2. Make sure you are on YOUR branch
   - Use "git checkout [branch_name]" to change to the correct branch.
   - If you want to use a new branch do "git checkout -b [branch_name]" instead.
      - After doing this, you will have to do "git push --set-upstream origin [branch_name]"
   - If you forgot the name of the branch do "git branch" and it will list all the branches.
3. Do "git add . --dry-run" and check that all the files listed are the ones you made changes to, otherwise double check your files.
4. If they are, do "git add ."
5. Then, do "git commit -m [message]" where the message you put should be in quotes.
6. Do "git pull"
   - You should not get any merge conflicts if you work only within your own branch.
   - If you do get merge conflicts, you'll need to figure out which files are conflicting and delete either one version of them.
8. Finally, do "git push"
