
## <a id="requirements"></a> Prerequisites

- Docker Engine 1.8+ on any supported Linux distribution or Docker for Mac/Windows. For more information, see [Install Docker](https://docs.docker.com/engine/installation/).
- Docker **overlay2** storage driver. This is the default for most users. If you find that you are not using this storage provider and need to change, please see the instructions and warnings in the [docker documentation for configuring overlay2](https://docs.docker.com/storage/storagedriver/overlayfs-driver/#configure-docker-with-the-overlay-or-overlay2-storage-driver).
- Minimum of 2 GB of disk space.
- Minimum of 2 GB of RAM.
- [System requirements for SQL Server on Linux](sql-server-linux-setup.md#system).

## <a id="pullandrun2019"></a> Pull and run the container image

1. Pull the SQL Server 2019 preview Linux container image from Docker Hub.

   ```bash
   sudo docker pull mcr.microsoft.com/mssql/server
   ```

   ```PowerShell
   docker pull mcr.microsoft.com/mssql/server
   ```
   
2. To run the container image with Docker, you can use the following command from a bash shell (Linux/macOS) or elevated PowerShell command prompt.

   ```bash
   docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=<YourStrong!Passw0rd>' \
      -p 1433:1433 --name sql1 \
      -d mcr.microsoft.com/mssql/server:2019-CTP2.4-ubuntu
   ```

   ```PowerShell
   docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=<YourStrong!Passw0rd>" `
      -p 1433:1433 --name sql1 `
      -d mcr.microsoft.com/mssql/server:2019-CTP2.4-ubuntu
   ```

   > [!NOTE]
   > The password should follow the SQL Server default password policy, otherwise the container can not setup SQL server and will stop working. By default, the password must be at least 8 characters long and contain characters from three of the following four sets: Uppercase letters, Lowercase letters, Base 10 digits, and Symbols. You can examine the error log by executing the [docker logs](https://docs.docker.com/engine/reference/commandline/logs/) command.

   > [!NOTE]
   > By default, this creates a container with the Developer edition of SQL Server 2019 preview.

   The following table provides a description of the parameters in the previous `docker run` example:

   | Parameter | Description |
   |-----|-----|
   | **-e 'ACCEPT_EULA=Y'** |  Set the **ACCEPT_EULA** variable to any value to confirm your acceptance of the [End-User Licensing Agreement](https://go.microsoft.com/fwlink/?LinkId=746388). Required setting for the SQL Server image. |
   | **-e 'SA_PASSWORD=\<YourStrong!Passw0rd\>'** | Specify your own strong password that is at least 8 characters and meets the [SQL Server password requirements](../relational-databases/security/password-policy.md). Required setting for the SQL Server image. |
   | **-p 1433:1433** | Map a TCP port on the host environment (first value) with a TCP port in the container (second value). In this example, SQL Server is listening on TCP 1433 in the container and this is exposed to the port, 1433, on the host. |
   | **--name sql1** | Specify a custom name for the container rather than a randomly generated one. If you run more than one container, you cannot reuse this same name. |
   | **mcr.microsoft.com/mssql/server:2019-CTP2.4-ubuntu** | The SQL Server 2019 CTP 2.4 Linux container image. |

3. To view your Docker containers, use the `docker ps` command.

   ```bash
   sudo docker ps -a
   ```

   ```PowerShell
   docker ps -a
   ```

   You should see output similar to the following screenshot:

   ![Docker ps command output](./media/sql-server-linux-setup-docker/docker-ps-command.png)

4. If the **STATUS** column shows a status of **Up**, then SQL Server is running in the container and listening on the port specified in the **PORTS** column. If the **STATUS** column for your SQL Server container shows **Exited**, see the [Troubleshooting section of the configuration guide](sql-server-linux-configure-docker.md#troubleshooting).

The `-h` (host name) parameter is also useful, but it is not used in this tutorial for simplicity. This changes the internal name of the container to a custom value. This is the name you'll see returned in the following Transact-SQL query:

```sql
SELECT @@SERVERNAME,
    SERVERPROPERTY('ComputerNamePhysicalNetBIOS'),
    SERVERPROPERTY('MachineName'),
    SERVERPROPERTY('ServerName')
```

Setting `-h` and `--name` to the same value is a good way to easily identify the target container.

## <a id="sapassword"></a> Change the SA password

<!-- This section was pasted in from includes/sql-server-linux-change-docker-password.md, to better support zone pivots. 2019/02/11 -->

The **SA** account is a system administrator on the SQL Server instance that gets created during setup. After creating your SQL Server container, the `MSSQL_SA_PASSWORD` environment variable you specified is discoverable by running `echo $MSSQL_SA_PASSWORD` in the container. For security purposes, change your SA password.

1. Choose a strong password to use for the SA user.

1. Use `docker exec` to run **sqlcmd** to change the password using Transact-SQL. Replace `<YourStrong!Passw0rd>` and `<YourNewStrong!Passw0rd>` with your own password values.

   ```bash
   sudo docker exec -it sql1 /opt/mssql-tools/bin/sqlcmd \
      -S localhost -U SA -P '<YourStrong!Passw0rd>' \
      -Q 'ALTER LOGIN SA WITH PASSWORD="<YourNewStrong!Passw0rd>"'
   ```

   ```PowerShell
   docker exec -it sql1 /opt/mssql-tools/bin/sqlcmd `
      -S localhost -U SA -P "<YourStrong!Passw0rd>" `
      -Q "ALTER LOGIN SA WITH PASSWORD='<YourNewStrong!Passw0rd>'"
   ```

## Connect to SQL Server

The following steps use the SQL Server command-line tool, **sqlcmd**, inside the container to connect to SQL Server.

1. Use the `docker exec -it` command to start an interactive bash shell inside your running container. In the following example `sql1` is name specified by the `--name` parameter when you created the container.

   ```bash
   sudo docker exec -it sql1 "bash"
   ```

   ```PowerShell
   docker exec -it sql1 "bash"
   ```

2. Once inside the container, connect locally with sqlcmd. Sqlcmd is not in the path by default, so you have to specify the full path.

   ```bash
   /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P '<YourNewStrong!Passw0rd>'
   ```

   > [!TIP]
   > You can omit the password on the command-line to be prompted to enter it.

3. If successful, you should get to a **sqlcmd** command prompt: `1>`.

## Create and query data

The following sections walk you through using **sqlcmd** and Transact-SQL to create a new database, add data, and run a simple query.

### Create a new database

The following steps create a new database named `TestDB`.

1. From the **sqlcmd** command prompt, paste the following Transact-SQL command to create a test database:

   ```sql
   CREATE DATABASE TestDB
   ```

2. On the next line, write a query to return the name of all of the databases on your server:

   ```sql
   SELECT Name from sys.Databases
   ```

3. The previous two commands were not executed immediately. You must type `GO` on a new line to execute the previous commands:

   ```sql
   GO
   ```

### Insert data

Next create a new table, `Inventory`, and insert two new rows.

1. From the **sqlcmd** command prompt, switch context to the new `TestDB` database:

   ```sql
   USE TestDB
   ```

2. Create new table named `Inventory`:

   ```sql
   CREATE TABLE Inventory (id INT, name NVARCHAR(50), quantity INT)
   ```

3. Insert data into the new table:

   ```sql
   INSERT INTO Inventory VALUES (1, 'banana', 150); INSERT INTO Inventory VALUES (2, 'orange', 154);
   ```

4. Type `GO` to execute the previous commands:

   ```sql
   GO
   ```

### Select data

Now, run a query to return data from the `Inventory` table.

1. From the **sqlcmd** command prompt, enter a query that returns rows from the `Inventory` table where the quantity is greater than 152:

   ```sql
   SELECT * FROM Inventory WHERE quantity > 152;
   ```

2. Execute the command:

   ```sql
   GO
   ```

### Exit the sqlcmd command prompt

1. To end your **sqlcmd** session, type `QUIT`:

   ```sql
   QUIT
   ```

2. To exit the interactive command-prompt in your container, type `exit`. Your container continues to run after you exit the interactive bash shell.

## <a id="connectexternal"></a> Connect from outside the container

You can also connect to the SQL Server instance on your Docker machine from any external Linux, Windows, or macOS tool that supports SQL connections.

The following steps use **sqlcmd** outside of your container to connect to SQL Server running in the container. These steps assume that you already have the SQL Server command-line tools installed outside of your container. The same principles apply when using other tools, but the process of connecting is unique to each tool.

1. Find the IP address for the machine that hosts your container. On Linux, use **ifconfig** or **ip addr**. On Windows, use **ipconfig**.

2. Run sqlcmd specifying the IP address and the port mapped to port 1433 in your container. In this example, that is the same port, 1433, on the host machine. If you specified a different mapped port on the host machine, you would use it here.

   ```bash
   sqlcmd -S 10.3.2.4,1433 -U SA -P '<YourNewStrong!Passw0rd>'
   ```

   ```PowerShell
   sqlcmd -S 10.3.2.4,1433 -U SA -P "<YourNewStrong!Passw0rd>"
   ```

3. Run Transact-SQL commands. When finished, type `QUIT`.

Other common tools to connect to SQL Server include:

- [Visual Studio Code](sql-server-linux-develop-use-vscode.md)
- [SQL Server Management Studio (SSMS) on Windows](sql-server-linux-manage-ssms.md)
- [Azure Data Studio](../azure-data-studio/what-is.md)
- [mssql-cli (Preview)](https://blogs.technet.microsoft.com/dataplatforminsider/2017/12/12/try-mssql-cli-a-new-interactive-command-line-tool-for-sql-server/)

## Remove your container

If you want to remove the SQL Server container used in this tutorial, run the following commands:

   ```bash
sudo docker stop sql1
sudo docker rm sql1
   ```

   ```PowerShell
docker stop sql1
docker rm sql1
   ```

> [!WARNING]
> Stopping and removing a container permanently deletes any SQL Server data in the container. If you need to preserve your data, [create and copy a backup file out of the container](tutorial-restore-backup-in-sql-server-container.md) or use a [container data persistence technique](sql-server-linux-configure-docker.md#persist).

## Docker demo

After you have tried using the SQL Server container image for Docker, you might want to know how Docker is used to improve development and testing. The following video shows how Docker can be used in a continuous integration and deployment scenario.

> [!VIDEO https://channel9.msdn.com/Events/Connect/2017/T152/player]

## Next steps

For a tutorial on how to restore database backup files into a container, see [Restore a SQL Server database in a Linux Docker container](tutorial-restore-backup-in-sql-server-container.md). To explore other scenarios, such as running multiple containers, data persistence, and troubleshooting, see [Configure SQL Server container images on Docker](sql-server-linux-configure-docker.md).

Also, check out the [mssql-docker GitHub repository](https://github.com/Microsoft/mssql-docker) for resources, feedback, and known issues.
