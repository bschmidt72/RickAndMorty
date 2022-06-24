import type {ResponseInfo} from "@/model/ResponseInfo";
import type {CharacterInfo} from "@/model/CharacterInfo";

export interface CharacterResponse {
    info: ResponseInfo,
    results: CharacterInfo[]
}