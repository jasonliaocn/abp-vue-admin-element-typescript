﻿<table width="95%" cellpadding="0" cellspacing="0" style="font-size: 11pt; font-family: Tahoma, Arial, Helvetica, sans-serif"> 
    <tr> 
        <td>
            <br /> <b><font color="#0B610B">{{ model.header }}</font></b> 
            <hr size="2" width="100%" align="center" />
        </td>
        </tr> 
        <tr> 
            <td> 
                <ul> 
                    <li>Type       &nbsp;：&nbsp;{{ model.type }}</li> 
                    <li>Message    &nbsp;：&nbsp;{{ model.message }}</li> 
                    <li>Alarm level&nbsp;：&nbsp;{{ model.loglevel }}</li> 
                    <li>TriggerTime&nbsp;：&nbsp;{{ creationTime }}</li> 
            </ul> 
        </td> 
    </tr> 
    <tr> 
        <td>
            <b><font color="#0B610B">Stack trace</font></b> 
            <hr size="2" width="100%" align="center" />
        </td> 
    </tr> 
    <tr> 
        <td>
            <pre style="font-size: 11pt; font-family: Tahoma, Arial, Helvetica, sans-serif">{{ model.stackTrace }}</pre> 
            <br />
        </td> 
    </tr> 
    <tr>
        <td>
            <br /> 
            <b style="float: right"><font color="#0B610B">{{ model.footer }}</font></b> 
            <hr size="2" width="100%" align="center" />
        </td>
    </tr>
</table> 