$(function(){
    var editor=document.getElementById("xml_editor");
    var regexLeafNode = {
        attributes: {
            "value": {},
            "caseinsensitive": {}
        }
    };

    var leafNode = {
        attributes: {
            "value": {}
        }
    };

    var docSpec = {
        elements: {
            "searchTree": {
                displayName: "Stalk search tree root"
            },
            "and": {
                caption: function() {return "Returns true all child nodes return true."}
            },
            "xor": {
                caption: function() {return "Returns true if exactly one child node returns true."}
            },
            "or": {
                caption: function() {return "Returns true any child node return true."}
            },
            "not": {
                caption: function() {return "Returns true if the child node returns false."}
            },
            "external": {
                caption: function() {return "References a section of tree stored elsewhere"}
            },
            "x-of": {
                attributes: {
                    "minimum": {},
                    "maximum": {}
                },
                caption: function() {return "Returns true of at least the minimum and at most the maximum child nodes return true."}
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
    Xonomy.render(editor.textContent, editor, docSpec);
});
