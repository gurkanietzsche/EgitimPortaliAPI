// Category Operations JavaScript

// Sayfa yüklendiğinde çalışacak fonksiyon
$(document).ready(function() {
    // Kategori yönetimi sayfasındaysa
    if (window.location.pathname.includes("CategoryManagement"))
    {
        loadCategories();
        setupCategoryEvents();
    }
});

// Kategorileri yükleyen fonksiyon
function loadCategories()
{
    $.ajax({
    url: '/Home/GetCategories',
        type: 'GET',
        dataType: 'json',
        success: function(response) {
            if (response.success)
            {
                displayCategories(response.data);
            }
            else
            {
                showAlert('danger', 'Kategoriler yüklenirken hata oluştu: ' + response.message);
            }
        },
        error: function(error) {
            showAlert('danger', 'API bağlantısı hatası: ' + error.statusText);
        }
    });
}

// Kategori detaylarını yükleyen fonksiyon
function loadCategoryDetails(categoryId)
{
    $.ajax({
    url: '/Home/GetCategory',
        type: 'GET',
        data: { id: categoryId },
        dataType: 'json',
        success: function(response) {
            if (response.success)
            {
                populateCategoryEditForm(response.data);
            }
            else
            {
                showAlert('danger', 'Kategori detayları yüklenirken hata oluştu: ' + response.message);
            }
        },
        error: function(error) {
            showAlert('danger', 'API bağlantısı hatası: ' + error.statusText);
        }
    });
}

// Yeni kategori ekleyen fonksiyon
function addCategory(categoryData)
{
    $.ajax({
    url: '/Home/AddCategory',
        type: 'POST',
        data: categoryData,
        success: function(response) {
            if (response.success)
            {
                showAlert('success', 'Kategori başarıyla eklendi.');
                loadCategories();
                $('#addCategoryModal').modal('hide');
            }
            else
            {
                showAlert('danger', 'Kategori eklenirken hata oluştu: ' + response.message);
            }
        },
        error: function(error) {
            showAlert('danger', 'API bağlantısı hatası: ' + error.statusText);
        }
    });
}

// Kategori güncelleyen fonksiyon
function updateCategory(categoryData)
{
    $.ajax({
    url: '/Home/UpdateCategory',
        type: 'POST',
        data: categoryData,
        success: function(response) {
            if (response.success)
            {
                showAlert('success', 'Kategori başarıyla güncellendi.');
                loadCategories();
                $('#editCategoryModal').modal('hide');
            }
            else
            {
                showAlert('danger', 'Kategori güncellenirken hata oluştu: ' + response.message);
            }
        },
        error: function(error) {
            showAlert('danger', 'API bağlantısı hatası: ' + error.statusText);
        }
    });
}

// Kategori silen fonksiyon
function deleteCategory(categoryId)
{
    if (confirm('Bu kategoriyi silmek istediğinizden emin misiniz?'))
    {
        $.ajax({
        url: '/Home/DeleteCategory',
            type: 'POST',
            data: { id: categoryId },
            success: function(response) {
                if (response.success)
                {
                    showAlert('success', 'Kategori başarıyla silindi.');
                    loadCategories();
                }
                else
                {
                    showAlert('danger', 'Kategori silinirken hata oluştu: ' + response.message);
                }
            },
            error: function(error) {
                showAlert('danger', 'API bağlantısı hatası: ' + error.statusText);
            }
        });
    }
}

// Kategori listesini gösteren fonksiyon
function displayCategories(categories)
{
    var container = $('#categoriesContainer');
    container.empty();

    if (categories && categories.length > 0)
    {
        var table = $('<table class="table table-striped">').append(
            '<thead>' +
            '<tr>' +
            '<th>ID</th>' +
            '<th>Ad</th>' +
            '<th>Açıklama</th>' +
            '<th>İşlemler</th>' +
            '</tr>' +
            '</thead>'
        );
        var tbody = $('<tbody>');

        $.each(categories, function(i, category) {
            var row = $('<tr>').append(
                '<td>' + category.id + '</td>' +
                '<td>' + category.name + '</td>' +
                '<td>' + (category.description || 'Açıklama yok') + '</td>' +
                '<td>' +
                '<button class="btn btn-sm btn-primary edit-category" data-id="' + category.id + '">Düzenle</button> ' +
                '<button class="btn btn-sm btn-danger delete-category" data-id="' + category.id + '">Sil</button>' +
                '</td>'
            );
            tbody.append(row);
        });

        table.append(tbody);
        container.append(table);
    }
    else
    {
        container.append('<div class="alert alert-info">Henüz kategori bulunmamaktadır.</div>');
    }
}

// Kategori düzenleme formunu dolduran fonksiyon
function populateCategoryEditForm(category)
{
    $('#editCategoryId').val(category.id);
    $('#editName').val(category.name);
    $('#editDescription').val(category.description);
    $('#editCategoryModal').modal('show');
}

// Kategori işlemlerinin eventlerini ayarlayan fonksiyon
function setupCategoryEvents()
{
    // Yeni kategori ekleme formu submit eventi
    $('#addCategoryForm').on('submit', function(e) {
        e.preventDefault();
        var categoryData = {
            name: $('#addName').val(),
            description: $('#addDescription').val()
        };
addCategory(categoryData);
    });

    // Kategori düzenleme formu submit eventi
    $('#editCategoryForm').on('submit', function(e) {
    e.preventDefault();
    var categoryData = {
            id: $('#editCategoryId').val(),
            name: $('#editName').val(),
            description: $('#editDescription').val()
        }
;
updateCategory(categoryData);
    });

    // Dynamically added button events using delegation
    $(document).on('click', '.edit-category', function() {
    var categoryId = $(this).data('id');
    loadCategoryDetails(categoryId);
});

    $(document).on('click', '.delete-category', function() {
    var categoryId = $(this).data('id');
    deleteCategory(categoryId);
});
}

// Uyarı mesajı gösteren yardımcı fonksiyon
function showAlert(type, message)
{
    var alertBox = $('<div class="alert alert-' + type + ' alert-dismissible fade show" role="alert">' +
        message +
        '<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>' +
        '</div>');
        
    $('#alertContainer').append(alertBox);

    // 5 saniye sonra otomatik kapat
    setTimeout(function() {
        alertBox.alert('close');
    }, 5000);
}