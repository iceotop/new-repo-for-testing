/*---------------------------------------------LOGIN/REGISTER STUFF---------------------------------------------*/

function submitLoginForm() {
  const username = document.getElementById('login-username').value;
  const password = document.getElementById('login-password').value;

  const data = {
    UserName: username,
    Password: password
  };

  fetch('http://localhost:5286/api/v1/account/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  }).then(response => {
    if (!response.ok) {
        throw new Error('Network response was not ok');
    }
    return response.json();
  }).then(data => {
    console.log(data)
    // Store the user details somewhere if needed
    document.cookie = `jwtToken=${data.token}; path=/;`;
    closeModal('loginModal');  // Close the login modal
    location.reload();  // Reload the page to apply new layout
  }).catch(error => {
    console.error('Error:', error);
  });
}

function LogOut() {
  document.cookie = 'jwtToken=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
  location.reload();
}

function submitRegistrationForm() {
  const username = document.getElementById('register-username').value;
  const password = document.getElementById('register-password').value;
  const email = document.getElementById('register-email').value;
  const firstName = document.getElementById('register-firstName').value;
  const lastName = document.getElementById('register-lastName').value;

  const data = {
    UserName: username,
    Password: password,
    Email: email,
    FirstName: firstName,
    LastName: lastName
  };

  fetch('http://localhost:5286/api/v1/account/register', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  }).then(response => {
    if (response.status === 201) {
      console.log("Registration successful");
      closeModal('registerModal'); // Close the registration modal
    } else if (response.status === 400) {
      throw new Error('Validation problem');
    } else {
      throw new Error('Network response was not ok');
    }
  }).catch(error => {
    console.error('Error:', error);
  });
}


//Open login/register modal
function openModal(modalId) {
  document.getElementById(modalId).style.display = "block";
}
//Close login/register modal
function closeModal(modalId) {
  document.getElementById(modalId).style.display = "none";

  if (modalId === 'loginModal') {
    document.getElementById('login-username').value = '';
    document.getElementById('login-password').value = '';
  } else if (modalId === 'registerModal') {
    document.getElementById('register-username').value = '';
    document.getElementById('register-password').value = '';
    document.getElementById('register-email').value = '';
    document.getElementById('register-firstName').value = '';
    document.getElementById('register-lastName').value = '';
  }
}

function parseJwt(token) {
  const base64Url = token.split('.')[1];
  const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
  const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
      return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
  }).join(''));

  return JSON.parse(jsonPayload);
};