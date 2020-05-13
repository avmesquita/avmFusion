# Fusion Room
<p>
<bloquote>Fusion Room is project to manage meeting rooms integrated with Microsoft Office 365 and a IoT device.</bloquote>
</p>

<h2>How it works?</h2>
<ol>
  <li>Turn on your IoT device</li>
  <li>Use a cellphone to connect to IOT device and configure your network configuration</li>
  <li>When your IoT is connected, it will send a command to our [Fusion.Exchange.Arduino] Web API Service to register serial number</li>
  <li>Now, need to access configuration page to complete registration at [Fusion.Exchange.Configuration] Asp.Net Mvc Portal. Here all configurations all done and connect your LDAP Meeting Room to our Services.</li>
  <li>One of our Services is Asp.Net Soap Service called [Fusion.Exchange.wcfsvcapp], it have rules to connect to MS O365 Exchange cloud server.</li>
  <li>There is a WebJob app which is a robot to observe all rooms status changes and take actions to alert meeting people if room is empty or canceling meeting room, let it avaiable into MS O365 LDAP to mark next meeting.</li>
</ol>
