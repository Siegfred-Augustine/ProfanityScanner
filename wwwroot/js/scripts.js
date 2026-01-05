const container = document.querySelector('.summary-list-item');

container.addEventListener('click', function (e) {
    // Check if the clicked element is one of the profane items
    console.log("CLICK");
    if (e.target.classList.contains('identified-profane-item')) {
        e.target.classList.toggle('hidden-content');
    }
});