module Main exposing (main)
import Browser
import Html exposing(..)
import Html.Attributes exposing(..)
import Html.Events exposing (onClick,onInput)
import Material.TopAppBar as TopAppBar  exposing( topAppBar, topAppBarConfig)
import Material.Button exposing (buttonConfig, textButton)
import Material.IconButton exposing( iconButton, iconButtonConfig)
import Material.Button exposing (buttonConfig, textButton)
import Material.List as MList exposing  ( list, listConfig, listItem, listItemConfig)
import Material.LayoutGrid as LayoutGrid exposing( layoutGrid, layoutGridCell, layoutGridInner)

-- main
main =
    Browser.sandbox {init=init,update=update,view=view}
-- model
type alias Model =
    {
        userName : String
        ,url : String
    }

init : Model
init =
    {
        userName = ""
        ,url = ""
    }

-- update

type Msg = Kerala 
        

update : Msg -> Model -> Model
update msg model =
    case msg of
        Kerala ->
            model
    



-- view
view :  Model -> Html Msg
view model =
    div[][
        appBar,
        listSelector
    ]

listSelector : Html Msg
listSelector =
    MList.list listConfig
        [ listItem listItemConfig [ text "Line item 1" ]
        , listItem listItemConfig [ text "Line item 2" ]
        , listItem listItemConfig [ text "Line item 3" ]
        ]


appBar : Html Msg
appBar =
    topAppBar topAppBarConfig
    [ TopAppBar.row []
        [ TopAppBar.section [ TopAppBar.alignStart ]
            [ iconButton
                { iconButtonConfig
                    | additionalAttributes =
                        [ TopAppBar.navigationIcon ]
                }
                "menu"
            , Html.span [ TopAppBar.title ]
                [ text "Demo" ]
            ]
        , TopAppBar.section [ TopAppBar.alignEnd ]
            [ textButton
                { buttonConfig
                    | additionalAttributes =
                        [ TopAppBar.actionItem ], onClick = Just Kerala
                }
                "SubText"
            ]
        ]
    ]
