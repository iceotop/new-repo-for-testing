let timeout;

// Reset the timer when user activity is detected
function resetTimer() {
  clearTimeout(timeout);

  // Log the user out after 15 minutes of inactivity
  timeout = setTimeout(LogOut, 15 * 60 * 1000);
}

// Redirect the user to the login page and remove their token
function LogOut() {
  document.cookie = 'jwtToken=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
  window.location.href = '/'; // or wherever your login/index page is
}

// Listen for any kind of activity to reset the timer
window.onload = resetTimer;
window.onmousemove = resetTimer;
window.onmousedown = resetTimer;
window.onclick = resetTimer;
window.onscroll = resetTimer;