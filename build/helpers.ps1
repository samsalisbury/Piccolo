function exec
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd
    )
    try {
		& $cmd
	} catch {
		write-output "##teamcity[buildStatus status='FAILURE' ]"
	}
}