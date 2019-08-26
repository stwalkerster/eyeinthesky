$(function () {
    var editor = document.getElementById("xml_editor");
    var xml = editor.textContent;

    var wrapFunction = function (htmlId, actionParameter) {
        var div = document.getElementById(htmlId);
        var jsElement = Xonomy.harvestElement(div);

        var newNode = Xonomy.xml2js(actionParameter);
        newNode.children = [jsElement];

        Xonomy.replace(htmlId, newNode);
    };

    var hideMenu = function (jsElement) {
        if (jsElement.name === "searchTree") {
            if (jsElement.children.length > 0) {
                return true;
            }
        }

        if (jsElement.name === "not" || jsElement.name === "expiry") {
            if (jsElement.children.length > 0) {
                return true;
            }
        }

        if (jsElement.name === "xor") {
            if (jsElement.children.length > 1) {
                return true;
            }
        }

        return false;
    };

    var addChildMenu = [
        {
            caption: "New node...",
            menu: [
                {
                    caption: "New leaf node...",
                    menu: [
                        {
                            caption: "Append <user>",
                            action: Xonomy.newElementChild,
                            actionParameter: "<user value=\"\" />",
                            hideIf: hideMenu
                        }, {
                            caption: "Append <page>",
                            action: Xonomy.newElementChild,
                            actionParameter: "<page value=\"\" />",
                            hideIf: hideMenu
                        }, {
                            caption: "Append <summary>",
                            action: Xonomy.newElementChild,
                            actionParameter: "<summary value=\"\" />",
                            hideIf: hideMenu
                        }, {
                            caption: "Append <log>",
                            action: Xonomy.newElementChild,
                            actionParameter: "<log value=\"\" />",
                            hideIf: hideMenu
                        }, {
                            caption: "Append <flag>",
                            action: Xonomy.newElementChild,
                            actionParameter: "<flag value=\"\" />",
                            hideIf: hideMenu
                        }, {
                            caption: "Append <incategory>",
                            action: Xonomy.newElementChild,
                            actionParameter: "<incategory value=\"\" />",
                            hideIf: hideMenu
                        }, {
                            caption: "Append <usergroup>",
                            action: Xonomy.newElementChild,
                            actionParameter: "<usergroup value=\"\" />",
                            hideIf: hideMenu
                        }
                    ],
                    hideIf: hideMenu
                },
                {
                    caption: "New logical node...",
                    menu: [
                        {
                            caption: "Append <and>",
                            action: Xonomy.newElementChild,
                            actionParameter: "<and />",
                            hideIf: hideMenu
                        }, {
                            caption: "Append <or>",
                            action: Xonomy.newElementChild,
                            actionParameter: "<or />",
                            hideIf: hideMenu
                        }, {
                            caption: "Append <not>",
                            action: Xonomy.newElementChild,
                            actionParameter: "<not />",
                            hideIf: hideMenu
                        }, {
                            caption: "Append <xor>",
                            action: Xonomy.newElementChild,
                            actionParameter: "<xor />",
                            hideIf: hideMenu
                        }, {
                            caption: "Append <x-of>",
                            action: Xonomy.newElementChild,
                            actionParameter: "<x-of />",
                            hideIf: hideMenu
                        }, {
                            caption: "Append <true>",
                            action: Xonomy.newElementChild,
                            actionParameter: "<true />",
                            hideIf: hideMenu
                        }, {
                            caption: "Append <false>",
                            action: Xonomy.newElementChild,
                            actionParameter: "<false />",
                            hideIf: hideMenu
                        }, {
                            caption: "Append <expiry>",
                            action: Xonomy.newElementChild,
                            actionParameter: "<expiry expiry='9999-12-31T23:59:59Z' />",
                            hideIf: hideMenu
                        }
                    ],
                    hideIf: hideMenu
                },
                {
                    caption: "Append <external>",
                    action: Xonomy.newElementChild,
                    actionParameter: "<external provider=\"phabricator\" location=\"\" />",
                    hideIf: hideMenu
                }
            ],
            hideIf: hideMenu
        }
    ];

    var wrapMenu = [
        {
            caption: "Wrap with logical node...",
            menu: [
                {
                    caption: "Wrap with <and>",
                    action: wrapFunction,
                    actionParameter: "<and />",
                    hideIf: hideMenu
                }, {
                    caption: "Wrap with <or>",
                    action: wrapFunction,
                    actionParameter: "<or />",
                    hideIf: hideMenu
                }, {
                    caption: "Wrap with <not>",
                    action: wrapFunction,
                    actionParameter: "<not />",
                    hideIf: hideMenu
                }, {
                    caption: "Wrap with <xor>",
                    action: wrapFunction,
                    actionParameter: "<xor />",
                    hideIf: hideMenu
                }, {
                    caption: "Wrap with <x-of>",
                    action: wrapFunction,
                    actionParameter: "<x-of />",
                    hideIf: hideMenu
                }, {
                    caption: "Wrap with <expiry />",
                    action: wrapFunction,
                    actionParameter: "<expiry expiry='9999-12-31T23:59:59Z' />",
                    hideIf: hideMenu
                }
            ],
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
                fromXml: function (xml) {
                    return xml;
                },
                toXml: function (text, origElement) {
                    return text.replace(/>\s*/g, '>').replace(/\s*</g, '<');
                }
            }
        }
    ];
    var addCommentMenu = [
        {
            caption: "Add comment",
            action: Xonomy.newAttribute,
            actionParameter: {name: "comment", value: ""},
            hideIf: function (jsElement) {
                return jsElement.hasAttribute("comment");
            }
        }
    ];
    
    var editableNodeMenu = [].concat(wrapMenu).concat(deleteItemMenu).concat(editRawMenu).concat(addCommentMenu);
    
    var canDropTo = ["searchTree", "and", "or", "x-of", "not", "xor", "expiry"];

    var commentAttribute = {
        asker: Xonomy.askString,
        menu: [{
            caption: "Remove comment",
            action: Xonomy.deleteAttribute
        }]
    };

    var defaultComments = {
        "and": "Returns true if all child nodes return true.",
        "xor": "Returns true if exactly one child node returns true.",
        "or": "Returns true if any child node return true.",
        "not": "Returns true if the child node returns false.",
        "external": "References a section of tree stored elsewhere",
        "x-of": "Returns true if at least the minimum and at most the maximum child nodes return true.",
        "expiry": "Returns false if the expiry has passed, otherwise evaluate subtree"
    };

    var commentFunc = function (node) {
        return "<span class=\"defaultcomment\">" + defaultComments[node.name] + "</span>";
    };
       

    var regexLeafNode = {
        menu: [{
            caption: "Mark as case insensitive",
            action: Xonomy.newAttribute,
            actionParameter: {name: "caseinsensitive", value: "true"},
            hideIf: function (jsElement) {
                return jsElement.hasAttribute("caseinsensitive");
            }
        }].concat(editableNodeMenu),
        attributes: {
            "value": {
                asker: Xonomy.askString
            },
            "caseinsensitive": {
                menu: [{
                    caption: "Mark as case sensitive",
                    action: Xonomy.deleteAttribute
                }]
            },
            "comment": commentAttribute
        },
        canDropTo: canDropTo,
        caption: commentFunc
    };

    var leafNode = {
        menu: editableNodeMenu,
        attributes: {
            "value": {
                asker: Xonomy.askString
            },
            "comment": commentAttribute
        },
        canDropTo: canDropTo,
        caption: commentFunc
    };

    var docSpec = {
        elements: {
            "searchTree": {
                menu: addChildMenu,
                displayName: "Stalk search tree root"
            },
            "and": {
                menu: addChildMenu.concat(editableNodeMenu),
                canDropTo: canDropTo,
                caption: commentFunc,
                attributes: {"comment": commentAttribute}
            },
            "xor": {
                menu: addChildMenu.concat(editableNodeMenu),
                canDropTo: canDropTo,
                caption: commentFunc,
                attributes: {"comment": commentAttribute}
            },
            "or": {
                menu: addChildMenu.concat(editableNodeMenu),
                canDropTo: canDropTo,
                caption: commentFunc,
                attributes: {"comment": commentAttribute}
            },
            "not": {
                menu: addChildMenu.concat(editableNodeMenu),
                canDropTo: canDropTo,
                caption: commentFunc,
                attributes: {"comment": commentAttribute}
            },
            "external": {
                menu: [].concat(editableNodeMenu),
                attributes: {
                    "provider": {
                        asker: Xonomy.askString
                    },
                    "location": {
                        asker: Xonomy.askString
                    },
                    "comment": commentAttribute
                },
                canDropTo: canDropTo,
                caption: commentFunc
            },
            "x-of": {
                menu: [
                    {
                        caption: "Add minimum",
                        action: Xonomy.newAttribute,
                        actionParameter: {name: "minimum", value: ""},
                        hideIf: function (jsElement) {
                            return jsElement.hasAttribute("minimum");
                        }
                    }, {
                        caption: "Add maximum",
                        action: Xonomy.newAttribute,
                        actionParameter: {name: "maximum", value: ""},
                        hideIf: function (jsElement) {
                            return jsElement.hasAttribute("maximum");
                        }
                    }
                ].concat(addChildMenu).concat(editableNodeMenu),
                attributes: {
                    "minimum": {
                        asker: Xonomy.askString,
                        menu: [{
                            caption: "Remove minimum",
                            action: Xonomy.deleteAttribute
                        }]
                    },
                    "maximum": {
                        asker: Xonomy.askString,
                        menu: [{
                            caption: "Remove maximum",
                            action: Xonomy.deleteAttribute
                        }]
                    },
                    "comment": commentAttribute
                },
                canDropTo: canDropTo,
                caption: commentFunc
            },
            "expiry": {
                menu: [].concat(addChildMenu).concat(editableNodeMenu),
                attributes: {
                    "expiry": {
                        asker: Xonomy.askString
                    },
                    "comment": commentAttribute
                },
                canDropTo: canDropTo,
                caption: commentFunc
            },

            "user": regexLeafNode,
            "page": regexLeafNode,
            "summary": regexLeafNode,
            "flag": regexLeafNode,
            "log": regexLeafNode,

            "usergroup": leafNode,
            "incategory": leafNode,

            "true": leafNode,
            "false": leafNode
        }
    };

    Xonomy.setMode("laic");
    Xonomy.render(xml, editor, docSpec);
});

$('#stalkForm').submit(function (event) {
    var xml = Xonomy.harvest();
    $('#newsearchtree').val(xml);
});