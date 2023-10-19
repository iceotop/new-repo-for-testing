let modalContext = ""; // This will hold either "log" or "circle"

document.addEventListener("DOMContentLoaded", function() {
    // Create and style the 'Log book' button
    const logBookButton = document.createElement("button");
    logBookButton.textContent = "Log book";
    logBookButton.className = "log-book-button";

    // Append the 'Log book' button to the bookReviewContainer
    document.getElementById("bookReviewContainer").appendChild(logBookButton);
    
    // Create and style the 'Create Book circle' button
    const createCircleButton = document.createElement("button");
    createCircleButton.textContent = "Create Book circle";
    createCircleButton.className = "create-circle-button";

    // Append the 'Create Book circle' button to the bookCircleContainer
    document.getElementById("bookCircleContainer").appendChild(createCircleButton);

    // Add click event to the 'Log book' button to show the first modal
    logBookButton.addEventListener("click", () => {
      modalContext = "log";
      showModal('modal1');
    });

    // Add click event to the 'Create Book circle' button (currently does nothing)
    createCircleButton.addEventListener("click", () => {
      modalContext = "circle";
      showModal('modal1');
    });

      // Close button for modal1
    const closeModal1 = document.getElementById("closeModal1");
    closeModal1.addEventListener("click", () => {
      hideModal("modal1");

    });

    // Close button for modal2
    const closeModal2 = document.getElementById("closeModal2");
    closeModal2.addEventListener("click", () => {
      hideModal("modal2");
    });

    // Login and register buttons for opening modals
    document.querySelector('.login').addEventListener('click', () => openModal('loginModal'));
    document.querySelector('.register').addEventListener('click', () => openModal('registerModal'));

    const searchForm = document.getElementById("search-form");
    const searchInput = document.getElementById("search-input");

    searchForm.addEventListener("submit", event => {
        event.preventDefault(); // Prevent the default form submission behavior

        const searchQuery = searchInput.value;

        if (searchQuery) {
            fetchBooks(searchQuery);  // Fetch books based on the query
        }
    });
});

// Function to fetch book data from your API
function fetchBooks(query) {
    const apiUrl = `http://localhost:5286/api/v1/externalservices/search?query=${encodeURIComponent(query)}`;
    
    fetch(apiUrl)
      .then(response => response.json())
      .then(data => {
        populateDropdown(data)
        console.log("Fetched Data:", data);
      })
      .catch(error => console.error('Fetch error :P:', error));
  }
  
  // Function to populate the dropdown
  function populateDropdown(data) {
    const dropdown = document.getElementById('resultsDropdown');
    dropdown.innerHTML = '';

    data.forEach(book => {
        const bookContainer = document.createElement('div');
        bookContainer.className = 'book-card';
        
        const bookImageDiv = document.createElement('div');
        bookImageDiv.className = 'book-image';
        const bookImage = document.createElement('img');
        bookImage.src = book.imageUrl;
        bookImage.alt = 'thumbnail';
        bookImageDiv.appendChild(bookImage);

        const bookInfoDiv = document.createElement('div');
        bookInfoDiv.className = 'book-info';

        const bookTitleDiv = document.createElement('div');
        bookTitleDiv.className = 'book-title';
        bookTitleDiv.textContent = book.title;

        const bookYearDiv = document.createElement('div');
        bookYearDiv.className = 'book-year';
        bookYearDiv.textContent = book.publicationYear ? book.publicationYear.substring(0, 4) : 'N/A';

        bookInfoDiv.appendChild(bookTitleDiv);
        bookInfoDiv.appendChild(bookYearDiv);
        bookContainer.appendChild(bookImageDiv);
        bookContainer.appendChild(bookInfoDiv);

        bookContainer.addEventListener('click', () => populateForm(book));
        
        dropdown.appendChild(bookContainer);
    });
  }

  // Function to populate the form in the second modal for book
  function populateForm(book) {
  
    if (modalContext === "log") {
      // Populate form for logging a book
      // Create and append form elements to formContainer
      // Same as your existing code for populating the form for logging a book
      document.getElementById('bookImage').src = book.imageUrl;
      document.getElementById('bookTitle').textContent = book.title;
      document.getElementById('bookYear').textContent = book.publicationYear ? book.publicationYear.substring(0, 4) : 'N/A';
      document.getElementById('bookAuthor').textContent = book.author;
  
    } else if (modalContext === "circle") {
      //Ta inte bort denna element, bara ta bort texten inom citat-tecknen så det blir tomt så här: "".
      formContainer.innerHTML = "Dynamiskt skapad formulär för bokcirkel ska hamna här";

      /* Skapa formulärelement dynamiskt här och glöm inte att utnyttja "book"-objektet 
       för att populera vissa fält i formuläret i modal2 innan den visas upp med showModal('modal2') där nere.*/
    }
    
    hideModal('modal1')
    showModal('modal2');
  }
  
  // Function to show modal
  function showModal(modalID) {
    document.getElementById(modalID).classList.add('show-modal');
    document.querySelector('.modal-overlay').classList.add('show-modal');
  }

  // Function to hide modal
  function hideModal(modalID) {
    document.getElementById(modalID).classList.remove('show-modal');
    document.querySelector('.modal-overlay').classList.remove('show-modal');

    // If it's modal1 being closed, clear search and dropdown
    if (modalID === 'modal1') {
      const searchInput = document.getElementById('search-input');
      const resultsDropdown = document.getElementById('resultsDropdown');

      searchInput.value = '';
      resultsDropdown.innerHTML = '';
    }
  }

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
      closeModal('loginModal');  // Close the login modal
    }).catch(error => {
      console.error('Error:', error);
    });
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
        return Promise.resolve("User created");
      } else if (!response.ok) {
        return response.json().then(data => {
          throw new Error('Network response was not ok');
        });
      }
    }).then(data => {
      if (data === "User created") {
        console.log(data);
        closeModal('registerModal');
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