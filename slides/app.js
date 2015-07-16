(function () {

    function calculateDistance(start, dest) {
        var xhr = new XMLHttpRequest();
        xhr.open('GET', 'http://localhost:7000/calc?start=' + start + '&dest=' + dest, true);
        xhr.setRequestHeader('Accept', 'application/json');
        xhr.onreadystatechange = function (e) {
            if (this.readyState === 4 && this.status === 200) {
                var data = JSON.parse(this.responseText);
                document.getElementById('result').textContent = data.distance || 'Could not calculate';
            }
        };
        xhr.send();
    }

    function calculateClick() {
        var start = document.getElementById('start').getAttribute('value');
        var dest = document.getElementById('dest').getAttribute('value');
        calculateDistance(start, dest);
    }

    document.getElementById('calculate').addEventListener('click', calculateClick, false);
})();
