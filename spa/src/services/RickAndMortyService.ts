import {config} from "@/config";
import type {CharacterInfo} from "@/model/CharacterInfo";
import axios from "axios"
import type {CharacterResponse} from "@/model/CharacterResponse";
import {Sanitizer} from "@/services/Sanitizer";
import type {Character} from "@/model/Character";
export class RickAndMortyService {
    
    private readonly characterApiUrl: string;
    private readonly sanitizer: Sanitizer;
    
    constructor() {
        this.characterApiUrl = config.apiUrl + 'character';
        this.sanitizer = Sanitizer.builder()
            .keepAlphaNumeric()
            .keepSpaces()
            .build();
    }
    
    public queryCharacterNames(search: string, abort: AbortController): Promise<CharacterInfo[]> {
        let sanitizedSearchString = this.sanitizer.sanitize(search);
        let url = this.characterApiUrl + '?name='+ sanitizedSearchString;
        let queryUrl = encodeURI(url);
        return RickAndMortyService.getQueryResults(queryUrl, abort);
    }
    
    public getCharacter(characterId: number): Promise<Character> {
        let url = this.characterApiUrl + '/' + characterId;
        return axios.get<Character>(url).then(result => result.data, error => error);
    }
    
    private static async getQueryResults(url: string, abort: AbortController): Promise<CharacterInfo[]> {
        let items = [] as CharacterInfo[];
        try {
            while (url && !abort.signal.aborted) {
                let response = await axios.get<CharacterResponse>(url, {signal: abort.signal})
                items.push(...response.data.results.map(r => {
                    return {id: r.id, name: r.name} as CharacterInfo
                }));
                url = response.data.info.next;
            }
        } catch (e) {
            console.log(e);
        }
        items.sort((a, b) => a.name.localeCompare(b.name));
        return items;
    }
    
    
}