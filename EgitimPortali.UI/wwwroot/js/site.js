// Site genelinde kullanılan JavaScript işlevleri

$(document).ready(function () {
    // Dropdown menülerin düzgün çalışması için
    $(".dropdown-toggle").dropdown();

    // Tooltips'i etkinleştir
    $('[data-toggle="tooltip"]').tooltip();

    // Çıkış yapma işlemi
    $("#logoutBtn").click(function (e) {
        e.preventDefault();
        $.ajax({
            type: "POST",
            url: "/Home/Logout",
            success: function (response) {
                if (response.success) {
                    window.location.href = "/";
                }
            }
        });
    });

    // Form doğrulama için genel fonksiyon
    $("[data-toggle='validator']").submit(function (e) {
        const form = $(this);
        if (form[0].checkValidity() === false) {
            e.preventDefault();
            e.stopPropagation();
        }
        form.addClass('was-validated');
    });

    // Sayfalama genel işlevleri
    if ($(".pagination").length) {
        $(".page-link").click(function (e) {
            if ($(this).parent().hasClass('disabled')) {
                e.preventDefault();
                return false;
            }
        });
    }

    // Resim yüklemeden önce önizleme gösterme
    $("input[type='file'][accept^='image']").change(function () {
        const file = this.files[0];
        if (file) {
            const reader = new FileReader();
            const preview = $(this).closest('.form-group').find('.image-preview');

            reader.onload = function (e) {
                if (preview.length) {
                    preview.attr('src', e.target.result);
                }
            }

            reader.readAsDataURL(file);
        }
    });

    // Email formatı doğrulama fonksiyonu
    window.validateEmail = function (email) {
        const re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(String(email).toLowerCase());
    }

    // AJAX hata gösterici
    $(document).ajaxError(function (event, jqXHR, settings, error) {
        console.error("AJAX Error:", error);
        if (jqXHR.status === 401) {
            alert("Oturumunuz sonlanmıştır. Lütfen tekrar giriş yapın.");
            window.location.href = "/Home/Login";
        } else if (jqXHR.status === 403) {
            alert("Bu işlemi yapmaya yetkiniz yok.");
        }
    });

    // Tabloların sıralanabilir olması için
    if ($(".sortable-table").length) {
        $(".sortable-table thead th").click(function () {
            const table = $(this).parents("table").eq(0);
            const rows = table.find("tr:gt(0)").toArray().sort(comparer($(this).index()));
            this.asc = !this.asc;
            if (!this.asc) {
                rows.reverse();
            }
            for (let i = 0; i < rows.length; i++) {
                table.append(rows[i]);
            }
        });
    }

    function comparer(index) {
        return function (a, b) {
            const valA = getCellValue(a, index);
            const valB = getCellValue(b, index);
            return $.isNumeric(valA) && $.isNumeric(valB) ? valA - valB : valA.localeCompare(valB);
        }
    }

    function getCellValue(row, index) {
        return $(row).children("td").eq(index).text();
    }
});

// Form verilerini JSON'a dönüştürme fonksiyonu
function formToJSON(form) {
    const array = $(form).serializeArray();
    const json = {};

    $.each(array, function () {
        json[this.name] = this.value || "";
    });

    return json;
}

// Para formatı fonksiyonu
function formatCurrency(amount) {
    return amount.toLocaleString('tr-TR', {
        style: 'currency',
        currency: 'TRY',
        minimumFractionDigits: 2
    });
}

// Tarih formatı fonksiyonu
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('tr-TR');
}

// Metin kısaltma fonksiyonu
function truncateText(text, maxLength) {
    if (!text) return "";
    return text.length > maxLength ? text.substring(0, maxLength) + "..." : text;
}