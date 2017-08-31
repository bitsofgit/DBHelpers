# DBHelpers
Utilities to monitor DB

In limited access enterprise environments, it becomes tough to monitor status, metrics etc. The purpose of these utilities is to work with a simple read-only access and to figure out certain stats.

# ConnectionTest.cs 
I wrote this class because I wanted to connect to 20-30 different databases to ensure that they are consistent in terms of connection setup and execution. So, what this class does is it takes a IConnectionInfo object with a command text and connection string - executes scalar on that connection string - and returns the time to open connection, execute a simple script, total time and return value. In case of error, an error message.

I would generally pass the command text as "SELECT GETDATE()" and use the times as a benchmark for future comparisons.

# AGLatencyTest.cs 
This class is trying to figure out if your Availability Group replicas are behind. SQL server provides lots of DMVs to check that status but if you dont have access to view server state or higher, there is no easy way to find this information out. So, what this class does is that it takes the connection string for primary and replica and executes a simple command at the same time in both DBs. If the values are same, the replica is in sync, otherwise not. The returned object contains the values in case they are different.

PrimaryName and ReplicaName are just helpful strings to identify DBs in case you have a lot of replicas or DBs.

I would generally pass the command text as "SELECT MAX(ID) FROM SomeHeavilyUsedTable" so that there is a good chance that this table is getting populated and therefore will give a correct sync status.

This is not a perfect solution but with limited read-only access - it gives a good enough info.
