var contextMenuVisible = false;
var contextMenuControlRef;
var controls = {};
var theme = new Theme();

$(document).ready(function () {
    setGeneralContextMenu();
    setThemeBindings();
});

$(document).keydown(function(e) {
    if(e.which == 46) {
        if(contextMenuControlRef) {
            deleteControl();
        }
    }
});

/********** Theme **********/
function themeClicked() {
    updateThemePanel();
    
    if (document.getElementById('theme').getBoundingClientRect().left >= $('body').innerWidth())
        $('#theme').transition({ x: -512 }, 300, 'easeInOutCubic');
    else 
        $('#theme').transition({ x: 0 }, 300, 'easeInOutCubic');
}
/********** Theme end **********/

/********** Controls List **********/
function toggleControlsList() {
    if (document.getElementById('controlsList').getBoundingClientRect().left >= $('body').innerWidth()) {
        updateList();
        $('#controlsList').transition({ x: -512 }, 300, 'easeInOutCubic');
    }
    else {
        $('#controlsList').transition({ x: 0 }, 300, 'easeInOutCubic');
    }
}

function updateList() {
    for (var key in controls) {
        if (!controls.hasOwnProperty(key)) continue;

        var control = controls[key];
        if (control.type === 'Container') {
            for (var i = 0; i < control.children.length; i++) {
                if (controls[control.children[i]]) continue;

                control.children.splice(i, 1);
                i--;
            }
        }
    }

    $('#controlsList').empty();
    populateList($('#siteArea').children(), 0);
    Waves.attach('.listControl');
}
/********** Controls List end **********/

/********** Buttons **********/
function buttonButtonClicked() {
    if (!contextMenuControlRef || !(controls[$(contextMenuControlRef).data('identifier')].type === 'Container')) {
        showSnackbar("Select a container");
        return;
    }

    $(contextMenuControlRef).append("<button class='floatingMaterialButton'>DEFAULT</button>");
    Waves.attach('.floatingMaterialButton', ['no-pointer']);

    var addedButton = $(contextMenuControlRef).children()[$(contextMenuControlRef).children().length - 1];

    var control = new ButtonControl();
    controls[control.identifier] = control;
    $(addedButton).data('identifier', control.identifier);
    updateColors(control);

    controls[$(contextMenuControlRef).data('identifier')].children.push(control.identifier);

    $(addedButton).click(function (e) {
        openMenu(controls[$(addedButton).data('identifier')].type, addedButton);
        e.stopPropagation();
    });
}

function divButtonClicked() {
    var addedDiv;
    var control = new ContainerControl();

    if (contextMenuControlRef && controls[$(contextMenuControlRef).data('identifier')].type === 'Container') {
        addedDiv = $("<div class='childDiv'></div>");
        $(contextMenuControlRef).append(addedDiv);
        control.color = "card";

        Waves.attach('.childDiv', ['no-pointer']);
    }
    else {
        addedDiv = $("<div class='defaultDiv'></div>");
        $('#siteArea').append(addedDiv);

        Waves.attach('.defaultDiv', ['no-pointer']);
    }

    addedDiv = addedDiv[0];
    $(addedDiv).data('identifier', control.identifier);
    controls[control.identifier] = control;
    updateColors(control);

    $(addedDiv).click(function (e) {
        openMenu(controls[$(addedDiv).data('identifier')].type, addedDiv);
        e.stopPropagation();
    });
}

function imageButtonClicked() {
    var addedImage;

    if (contextMenuControlRef && controls[$(contextMenuControlRef).data('identifier')].type === 'Container') {
        addedImage = $('<img src="../images/default_image.jpg" class="defaultImage"/>');
        $(contextMenuControlRef).append(addedImage);

        Waves.attach('.defaultImage', ['no-pointer']);
    }
    else {
        addedImage = $('<img src="../images/default_image.jpg" class="fullImage"/>');
        $('#siteArea').append(addedImage);

        Waves.attach('.fullImage', ['no-pointer']);
    }
    
    addedImage = addedImage.parent()[0];
    var control = new ImageControl();
    $(addedImage).data('identifier', control.identifier);
    controls[control.identifier] = control;
    updateColors(control);

    $(addedImage).click(function (e) {
        openMenu(controls[$(addedImage).data('identifier')].type, addedImage);
        e.stopPropagation();
    });
}

function labelButtonClicked() {
    if (!contextMenuControlRef || !(controls[$(contextMenuControlRef).data('identifier')].type === 'Container')) {
        showSnackbar("Select a container");
        return;
    }

    $(contextMenuControlRef).append("<p class='defaultLabel'>Text</p>");
    Waves.attach('.defaultLabel', ['no-pointer']);

    var addedLabel = $(contextMenuControlRef).children()[$(contextMenuControlRef).children().length - 1];

    var control = new LabelControl();
    controls[control.identifier] = control;
    $(addedLabel).data('identifier', control.identifier);
    updateColors(control);

    controls[$(contextMenuControlRef).data('identifier')].children.push(control.identifier);

    $(addedLabel).click(function (e) {
        openMenu(controls[$(addedLabel).data('identifier')].type, addedLabel);
        e.stopPropagation();
    });
}