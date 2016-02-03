[CmdletBinding()]
param (
    [switch] $recreate,
    [switch] $drop
)

# to see what you have installed, uncomment and run:
#$sqlcmd = cmd /c where sqlcmd; ls $sqlcmd -r | % versioninfo

function runSql ($ex){
    $ex = "sqlcmd -S '(localdb)\v11.0' -d 'master' $ex"
    echo $ex
    $ret =  iex $ex
    echo $ret
    if(-not ($ret -match "rows affected" -or $ret -eq $null)){
        exit 1
    }
}

if ($recreate){
    echo 'recreating the database'
    $sf = "$PSScriptRoot\create-db.sql"
    runSql "-i '$sf'"
}

if ($drop){
    $database = 'Database1'
    echo 'dropping the database'
    # drop any connections first
    runSql "-Q 'alter database $database set single_user with rollback immediate'"
    runSql "-Q 'use master; drop database $database'"
}

exit 0
