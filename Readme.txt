RoboMirror Readme v0.5

Martin Kinkelin, 17.01.2009



What is RoboMirror?
---
RoboMirror is a GUI front-end for the quite popular Robocopy command-line
utility for Windows.
Robocopy is shipped with Windows Vista and available as free SDK tool for
previous Windows versions (RoboMirror already includes it).
Robocopy allows to robustly synchronize two directory trees, both locally
and across the network. It is therefore often used for mirroring purposes,
e.g. as a powerful yet free backup utility.
RoboMirror's aim is to make Robocopy more accessible by providing a very
simple and user-centered GUI, because simply most users do not know how to
operate the shell.
RoboMirror also supports volume shadow copies (from Vista upwards only).
By using shadow copies, all files can be copied, including ones locked by
running processes (Outlook, SQL Server...).

What are its features?
---
1) Very simple interface.
   Simplicity was one of the primary goals as I started coding this little
   application when my mum asked me for a nice backup tool. As I was not
   able to find something I was looking for, I started this project with
   the aim to be as simple as possible while still providing for what
   most users need, e.g. performing backups of folders to external hard
   drives or network shares, synchronizing an MP3 player with the music
   folder etc.
2) Management of mirror tasks, defined by a source and a target folder.
   When performing a backup, the target folder will be synchronized to the
   state of the source folder, e.g. be a mirror of the source folder.
   When performing a restore, the operation is reversed, i.e. the source
   folder will be a mirror of the target folder.
3) Support for automatic task scheduling.
   Mirror tasks may be scheduled on a regular basis (daily, weekly and
   monthly at a given time).
   The Robocopy output will be stored as an entry in the application
   event log.
4) "Preview run" to identify the pending changes which are then presented
   to the user, asking for confirmation. This should prevent unpleasant
   surprises. ;)
5) Support for exclusions.
   Entire subfolders, files and wildcards may be excluded from
   synchronization. These items are treated as if they would not exist, so
   they are not copied and matching existing items in the destination
   folder are deleted.
   Additionally, files may be excluded by any of the common attributes
   (Hidden, System and Encrypted).
6) Support for extended attributes.
   The user can choose to copy only data, attributes and time stamps of the
   files or to also copy extended attributes, either only the access
   control lists or everything (ACLs, owner and auditing information).
7) Support for volume shadow copies (from Vista upwards only).
   By creating a temporary shadow copy of the source folder and operating
   on that shadow copy, all files can be copied, including ones locked by
   running processes (e.g. Outlook locking its .pst files, SQL server its
   databases).
   This is accomplished using another free utility, published by Microsoft
   in its Windows SDK. RoboMirror is bundled with both x86 and AMD64
   versions. RoboMirror uses a persistent shadow copy to be able to get
   ahold of Robocopy's error code, and those persistent shadow copies
   are only supported since Windows Vista.

Known limitations/annoyances
---
Each text entry in the event log cannot exceed a total of about 31850
characters. This may cause incomplete log entries if many files are
copied.
RoboMirror needs administrator rights, possibly causing a UAC pop-up
on Vista every time it is started. This is due to advanced Robocopy
options and shadow copy creation.

What are the default Robocopy options used?
---
/mir /b /xj /r:1 /w:5 /tbd /np
Nothing special here; mirroring in backup mode, excluding NTFS junctions
(soft links; excluded because they may cause recursions) and 1 retry
attempt after 5 seconds if a file could not be copied (possibly due to a
network error or a locking process). You may modify the options to your
liking by editing the file "RoboMirror.exe.config" in the application
directory (e.g. "C:\Program Files\RoboMirror"). It is an XML file, open it
with your favourite text editor.
In that file you may also disable the "preview run" if you feel confident
and want to spare some seconds and a mouse click when performing a backup.
