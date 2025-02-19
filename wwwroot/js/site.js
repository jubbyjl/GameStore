// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// User balance layout display
function getBal() {
    fetch('/account/userbalance', { credentials: 'include' })
        .then(res => res.json())
        .then(data => _displayBal(data))
        .catch(err => console.error(err));
}

function _displayBal(bal) {
    document.getElementById('userbalance').textContent = '$' + bal.toFixed(2);
}

document.getElementById('userbalance').onload = getBal();
