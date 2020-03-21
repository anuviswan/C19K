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
import LineChart
import LineChart.Dots as Dots
import LineChart.Colors as Colors
import LineChart.Interpolation as Interpolation
import Parts.Districts as Districts exposing (districtDecoder)


-- main
main =
    Browser.sandbox {init=init,update=update,view=view}
-- model
type alias Model =
    {
        day : Float
        ,count : Int
    }


type alias Info =
  { day : Float
  , count : Float
  }


init : Model
init =
    {
         day = 0
        ,count = 0
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
    div[]
    [
        appBar
        ,div[][text "SAdasd"]
        ,layoutGrid []
            [ layoutGridInner []
                [ 
                    layoutGridCell [LayoutGrid.span1 ] [listSelector]
                    ,layoutGridCell [] [chart model]
                    , layoutGridCell [][]
                ]
          
            ]
    ]
    
type alias Point =
  { x : Float, y : Float }

getListItem : String -> MList.ListItem msg
getListItem itemText =
    listItem listItemConfig [text itemText]
    

listSelector : Html Msg
listSelector =
    MList.list listConfig
        (List.map getListItem ["Kerala","Ernakulam","Allepy"])
     ---(List.repeat 3 <| listItem listItemConfig [ text "Kerala" ])

     


chart : Model -> Html Msg
chart model =
  LineChart.view .day .count
    [ LineChart.line Colors.red Dots.diamond "Alice" alice
    ]

alice : List Info
alice =
  [ Info 1.0 14
  , Info 2 14 
  , Info 3 16 
  , Info 4 16
  , Info 5 19 
  , Info 6 19
  , Info 7 21
  , Info 8 24
  , Info 9 24
  , Info 10 24
  ]




appBar : Html Msg
appBar =
    div[]
    [
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
                [ text "C19.Kerala" ]
            ]
        , TopAppBar.section [ TopAppBar.alignEnd ]
            [ text "Kerala"
            ]
        ]
    ]
    ]
