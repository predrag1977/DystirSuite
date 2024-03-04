import React, { useEffect, useState } from "react";
import { 
    FlatList, 
    SafeAreaView, 
    StatusBar, 
    StyleSheet,
} from "react-native";
import { loadMatchesDataAsync } from "../App";
import MatchComponent from "../components/MatchComponent";

const styles = StyleSheet.create({
    container: {
        flex: 1,
        paddingTop: 22,
    },
});
  
function MatchesScreen () {
    const [matches, setMatches] = useState<any>([]);

    useEffect(() => {
        loadMatchesDataAsync()
        .then(data => {
            setMatches(data)
        }
    )}, []);
    return (
        <SafeAreaView style={styles.container}>
            <StatusBar/>
            <FlatList
                data={matches}
                renderItem={({item}) => 
                <MatchComponent match={item}/>
            }
            />
        </SafeAreaView>
    )
}

export default MatchesScreen