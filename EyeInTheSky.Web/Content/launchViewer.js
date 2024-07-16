$(function () {
    var editor = document.getElementById("xml_editor");

    var defaultComments = {
        "and": "Returns true if all child nodes return true.",
        "xor": "Returns true if exactly one child node returns true.",
        "or": "Returns true if any child node return true.",
        "not": "Returns true if the child node returns false.",
        "x-of": "Returns true if at least the minimum and at most the maximum child nodes return true.",
        "expiry": "Returns false if the expiry has passed, otherwise evaluate subtree"
    };

    var commentFunc = function (node) {
        if (node.hasAttribute("comment")) {
            return "<span class=\"usercomment\">" + node.attributes.filter(function (x) {
                return x.name === "comment"
            })[0].value + "</span>";
        } else {
            return "<span class=\"defaultcomment\">" + defaultComments[node.name] + "</span>";
        }
    };

    var regexLeafNode = {
        caption: commentFunc,
        attributes: {
            "value": {},
            "caseinsensitive": {},
            "comment": {isInvisible: true}
        }
    };

    var leafNode = {
        caption: commentFunc,
        attributes: {
            "value": {},
            "comment": {isInvisible: true}
        }
    };

    var commentAttribute = {"comment": {isInvisible: true}};

    var docSpec = {
        elements: {
            "searchTree": {
                displayName: "Stalk search tree root"
            },
            "and": {
                caption: commentFunc,
                attributes: commentAttribute
            },
            "xor": {
                caption: commentFunc,
                attributes: commentAttribute
            },
            "or": {
                caption: commentFunc,
                attributes: commentAttribute
            },
            "not": {
                caption: commentFunc,
                attributes: commentAttribute
            },
            "x-of": {
                attributes: {
                    "minimum": {},
                    "maximum": {},
                    "comment": {isInvisible: true}
                },
                caption: commentFunc
            },
            "expiry": {
                attributes: {
                    "expiry": {},
                    "comment": {isInvisible: true}
                },
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
    Xonomy.render(editor.textContent, editor, docSpec);
});
