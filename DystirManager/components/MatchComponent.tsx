import { PropsWithChildren } from "react";
import { StyleSheet, Text, View } from "react-native"

type MatchComponentProps = PropsWithChildren<{
    match: any;
}>;

const styles = StyleSheet.create({
    container: {
      flex: 1,
      paddingTop: 22,
    },
    item: {
      padding: 10,
      fontSize: 18,
      height: 44,
    },
  });

function MatchComponent({match} : MatchComponentProps) : React.JSX.Element {
    return (
        <View>
            <Text style={styles.item}>{match.time} - {match.location}</Text>
            <Text style={styles.item}>{match.homeTeam} - {match.awayTeam}</Text>
            <Text style={styles.item}>{match.homeTeamScore} : {match.awayTeamScore}</Text>
        </View>
    )
}

export default MatchComponent