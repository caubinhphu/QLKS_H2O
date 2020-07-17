document
    .getElementById('btnthemphong')
    .addEventListener('click', function () {
        const select = document.createElement('select');
        select.name = 'phong';
        select.className = 'form-control form-control-sm';
        select.innerHTML = document.getElementById('phong').innerHTML;
        const input = document.createElement('input');
        input.type = 'number';
        input.name = 'songuoi';
        input.value = 1;
        input.className = 'form-control form-control-sm';
        const button = document.createElement('button');
        button.className = 'btn btn-sm btn-danger';
        button.innerHTML = 'X';
        button.addEventListener('click', function () {
            this.parentElement.remove();
        });
        const divChild = document.createElement('div');
        divChild.className = 'd-flex';
        divChild.appendChild(select);
        divChild.appendChild(input);
        divChild.appendChild(button);

        document.getElementById('thuephong').appendChild(divChild);
    });


document
    .getElementById('btnthemdichvu')
    .addEventListener('click', function () {
        const select = document.createElement('select');
        select.name = 'dichvu';
        select.className = 'form-control form-control-sm';
        select.innerHTML = document.getElementById('dichVus').innerHTML;
        const input = document.createElement('input');
        input.type = 'number';
        input.name = 'soluong';
        input.value = 1;
        input.className = 'form-control form-control-sm';
        const button = document.createElement('button');
        button.className = 'btn btn-sm btn-danger';
        button.innerHTML = 'X';
        button.addEventListener('click', function () {
            this.parentElement.remove();
        });
        const divChild = document.createElement('div');
        divChild.className = 'd-flex';
        divChild.appendChild(select);
        divChild.appendChild(input);
        divChild.appendChild(button);

        document.getElementById('thuedichvu').appendChild(divChild);
    });