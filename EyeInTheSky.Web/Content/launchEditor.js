$(function(){
    var editor=document.getElementById("xml_editor");
    var xml = editor.textContent;

    var hideMenu = function(jsElement){
        if(jsElement.name === "searchTree") {
            if(jsElement.children.length > 0) {
                return true;
            }
        }

        if(jsElement.name === "not") {
            if(jsElement.children.length > 0) {
                return true;
            }
        }

        if(jsElement.name === "xor") {
            if(jsElement.children.length > 1) {
                return true;
            }
        }

        return false;
    };

    var addChildMenu = [
        {
            caption: "Append <and>",
            action: Xonomy.newElementChild,
            actionParameter: "<and />",
            hideIf: hideMenu
        },{
            caption: "Append <or>",
            action: Xonomy.newElementChild,
            actionParameter: "<or />",
            hideIf: hideMenu
        },{
            caption: "Append <not>",
            action: Xonomy.newElementChild,
            actionParameter: "<not />",
            hideIf: hideMenu
        },{
            caption: "Append <xor>",
            action: Xonomy.newElementChild,
            actionParameter: "<xor />",
            hideIf: hideMenu
            // },{
            //     caption: "Append <x-of>",
            //     action: Xonomy.newElementChild,
            //     actionParameter: "<x-of />",
            //     hideIf: hideMenu
        },{
            caption: "Append <user>",
            action: Xonomy.newElementChild,
            actionParameter: "<user value=\"\" />",
            hideIf: hideMenu
        },{
            caption: "Append <page>",
            action: Xonomy.newElementChild,
            actionParameter: "<page value=\"\" />",
            hideIf: hideMenu
        },{
            caption: "Append <summary>",
            action: Xonomy.newElementChild,
            actionParameter: "<summary value=\"\" />",
            hideIf: hideMenu
        },{
            caption: "Append <log>",
            action: Xonomy.newElementChild,
            actionParameter: "<log value=\"\" />",
            hideIf: hideMenu
        },{
            caption: "Append <flag>",
            action: Xonomy.newElementChild,
            actionParameter: "<flag value=\"\" />",
            hideIf: hideMenu
        },{
            caption: "Append <incategory>",
            action: Xonomy.newElementChild,
            actionParameter: "<incategory value=\"\" />",
            hideIf: hideMenu
        },{
            caption: "Append <usergroup>",
            action: Xonomy.newElementChild,
            actionParameter: "<usergroup value=\"\" />",
            hideIf: hideMenu
        }
    ];

    var deleteItemMenu = [
        {
            caption: "Delete entry",
            action: Xonomy.deleteElement
        }
    ];

    var editRawMenu = [
        {
            caption: "Edit raw",
            action: Xonomy.editRaw,
            actionParameter: {
                fromXml: function(xml) {return xml;},
                toXml: function(text, origElement) { return text;}
            }
        }
    ];

    var canDropTo = ["searchTree", "and", "or", "x-of", "not", "xor"];

    var regexLeafNode = {
        menu: [{
                caption: "Mark as case insensitive",
                action: Xonomy.newAttribute,
                actionParameter: {name: "caseinsensitive", value: "true"},
                hideIf: function(jsElement) {
                    return jsElement.hasAttribute("caseinsensitive");
                }
            }].concat(deleteItemMenu).concat(editRawMenu),
    attributes: {
        "value": {
            asker: Xonomy.askString
        },
        "caseinsensitive": {
            menu: [{
                caption: "Mark as case sensitive",
                action: Xonomy.deleteAttribute
            }]
        }
    },
    canDropTo: canDropTo
};

    var leafNode = {
        menu: deleteItemMenu.concat(editRawMenu),
        attributes: {
            "value": {
                asker: Xonomy.askString
            }
        },
        canDropTo: canDropTo
    };

    var docSpec = {
        elements: {
            "searchTree": {
                menu: addChildMenu,
                displayName: "Stalk search tree root"
            },
            "and": {
                menu: addChildMenu.concat(deleteItemMenu).concat(editRawMenu),
                canDropTo: canDropTo
            },
            "or": {
                menu: addChildMenu.concat(deleteItemMenu).concat(editRawMenu),
                canDropTo: canDropTo
            },
            "not": {
                menu: addChildMenu.concat(deleteItemMenu).concat(editRawMenu),
                canDropTo: canDropTo
            },
            "user": regexLeafNode,
            "page": regexLeafNode,
            "summary": regexLeafNode,
            "flag": regexLeafNode,
            "log": regexLeafNode,

            "usergroup": leafNode,
            "incategory": leafNode
        },
        // allowModeSwitching: true
    };

    Xonomy.setMode("laic");
    Xonomy.render(xml, editor, docSpec);
});
