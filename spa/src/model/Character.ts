import type { CharacterLocation } from "@/model/Location";

export interface Character {
  name: string;
  status: string;
  image: string;
  location: CharacterLocation;
}
