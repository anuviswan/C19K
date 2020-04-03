module Parts.Districts exposing (districtDecoder)
import Http
import Json.Decode as Decode exposing (Decoder, field, string,list,index)

type Model
  = Failure
  | Loading
  | Success String


init : () -> (Model, Cmd Msg)
init _ =
  (Loading, getDistrictList)


type Msg
  = GetData
  | GotData (Result Http.Error String)

getDistrictList : Cmd Msg
getDistrictList =
  Http.get
    { url = "https://localhost:44384/api/general/getdistricts"
    , expect = Http.expectJson GotData districtDecoder
    }


--districtDecoder : Decoder String
districtDecoder =
  field "Districts" (field "Name" decodeList) 

--decodeList : Decoder (List String)
decodeList =
  field "name" string