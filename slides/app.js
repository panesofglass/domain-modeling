(function () {

    function calculateDistance(start, dest) {
        var xhr = new XMLHttpRequest();
        xhr.open('GET', 'http://localhost:7000/calc?start=' + encodeURIComponent(start) + '&dest=' + encodeURIComponent(dest), true);
        xhr.setRequestHeader('Accept', 'application/json');
        xhr.onreadystatechange = function (e) {
            if (this.readyState === 4 && this.status === 200) {
                var data = JSON.parse(this.responseText);
                document.getElementById('show-result').value = data.distance || 'Could not calculate';
            }
        };
        xhr.send();
    }

    function calculateClick() {
        var start = document.getElementById('start').value;
        var dest = document.getElementById('dest').value;
        calculateDistance(start, dest);
    }

    document.getElementById('calculate').addEventListener('click', calculateClick, false);
})();
