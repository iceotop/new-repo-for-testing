var connection = new signalR.HubConnectionBuilder().withUrl('/chathub').build();

// Eventlistener som lyssnar efter inkommande meddelanden
connection.on('ReceiveMessage', function (user, message) {
  var li = document.createElement('li');
  li.textContent = user + ': ' + message;
  document.getElementById('messages').appendChild(li);
});

// Eventlistener "skicka"
document.getElementById('btnSend').addEventListener('click', function (event) {
  var user = document.getElementById('txtUser').value;
  var message = document.getElementById('txtMessage').value;
  // Skicka meddelandet till servern
  connection.invoke('SendMessage', user, message).catch(function (err) {
    return console.error(err.toString());
  });
  event.preventDefault();
});

connection
  .start()
  .then(function () {
    console.log('SignalR connected!');
  })
  .catch(function (err) {
    return console.error(err.toString());
  });
