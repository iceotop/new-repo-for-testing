document.addEventListener("DOMContentLoaded", function() {
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
      .catch(error => console.error('Fetch error:', error));
  }
  
  // Function to populate the dropdown
  function populateDropdown(data) {
    const dropdown = document.getElementById('resultsDropdown');
    dropdown.innerHTML = '';

    data.forEach(book => {
      const resultDiv = document.createElement('div');
      resultDiv.innerHTML = `
        <img src="${book.imageUrl}" alt="thumbnail">
        <span>${book.title}</span>
        <span>${book.publicationYear}</span>
        <span>${book.author}</span>
      `;

      resultDiv.addEventListener('click', () => populateForm(book));
      dropdown.appendChild(resultDiv);
    });
  }

  // Function to populate the form in the second modal
  function populateForm(book) {
    document.getElementById('bookImage').src = book.imageUrl;
    document.getElementById('bookTitle').value = book.title;
    document.getElementById('bookYear').value = book.publicationYear;
    document.getElementById('bookAuthor').value = book.author;

    // Close the first modal and open the second
    closeFirstModal();
    openSecondModal();
  }
  
  // Event listeners and modal handling logic here
  // ...
  


// document.addEventListener("DOMContentLoaded", function() {
//   const searchForm = document.getElementById("search-form");
//   const searchInput = document.getElementById("search-input");

//   searchForm.addEventListener("submit", event => {
//       event.preventDefault(); // Prevent the default form submission behavior

//       const searchQuery = searchInput.value;

//       if (searchQuery) {
//           fetchBooks(searchQuery);  // Fetch books based on the query
//       }
//   });
// });

// // Function to fetch book data from your API
// function fetchBooks(query) {
//   const apiUrl = `http://localhost:5286/api/v1/externalservices/search?query=${encodeURIComponent(query)}`;
  
//   fetch(apiUrl)
//     .then(response => response.json())
//     .then(data => {
//         // Commented out the function that populates the dropdown
//         // populateDropdown(data);

//         // Logging the fetched data to console for debugging
//         console.log("Fetched Data:", data);
//     })
//     .catch(error => console.error('Fetch error:', error));
// }