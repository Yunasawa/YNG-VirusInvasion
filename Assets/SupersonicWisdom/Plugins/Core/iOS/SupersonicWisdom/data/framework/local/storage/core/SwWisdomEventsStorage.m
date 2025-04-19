//
//  SwWisdomEventsStorage.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/12/2020.
//

#import "SwWisdomEventsStorage.h"

@implementation SwWisdomEventsStorage {
    SwWisdomDBManager *dbManager;
}

- (id)initWithDatabase:(SwWisdomDBManager *)databaseManager {
    if (!(self = [super init])) return nil;
    dbManager = databaseManager;
    return self;
}

- (NSInteger)insertInto:(NSString *)table listWithValues:(NSArray *)list {
    NSInteger rows = 0;
    for (NSDictionary *dict in list) {
        if ([self insertInto:table values:dict] > -1) {
            rows++;
        }
    }
    
    return rows;
}

- (NSInteger)insertInto:(NSString *)table values:(NSDictionary *)values {
    NSArray *keys = [values allKeys];
    NSString *columns = [self createColumnsString:keys];
    
    return [dbManager insertInto:table forColumns:columns withValues:values];
}

- (NSArray *)queryFrom:(NSString *)table orderBy:(NSString *)orderBy limit:(NSInteger)limit {
    return [dbManager selectFrom:table columns:@"*" where:@"" orderBy:orderBy limit:limit];
}

- (NSInteger)updateIn:(NSString *)table whereClause:(NSString *)whereClause whereArgs:(NSArray *)columns listWithValues:(NSArray *)list {
    if (!list || [list count] == 0) {
        return 0;
    }
    
    int res = 0;
    NSString *condition = [NSString string];
    for (NSDictionary *dict in list) {
        for (NSString *column in columns) {
            NSRange range = [whereClause rangeOfString:@"?"];
            NSString *value = [[dict objectForKey:column] description];
            condition = [whereClause stringByReplacingCharactersInRange:range withString:value];
        }
        NSString *set = [self dictToString:dict];
        res += [dbManager updateIn:table set:set where:condition];
    }
    
    return res;
}

- (NSInteger)deleteFrom:(NSString *)table whereClause:(NSString *)whereClause whereArgs:(NSArray *)whereArgs {
    for (NSString *arg in whereArgs) {
        whereClause = [whereClause stringByReplacingCharactersInRange:[whereClause rangeOfString:@"?"] withString:[arg description]];
    }
    return [dbManager deleteFrom:table where:whereClause];
}

- (NSInteger)deleteAllFrom:(NSString *)table {
    return [dbManager deleteFrom:table where:nil];
}

- (NSString *)dictToString:(NSDictionary *)dict {
    NSArray *keys = [dict allKeys];
    NSString *str = [NSString string];
    NSString *result = [NSString string];
    int count = 0;
    for (NSString *key in keys) {
        count++;
        str = [NSString stringWithFormat:@"%@ = '%@'",key, [dict objectForKey:key]];
        if (count < keys.count) {
            str = [str stringByAppendingString:@", "];
        }
        
        result = [result stringByAppendingString:str];
    }
    
    return result;
}

- (NSString *)createColumnsString:(NSArray *)keys {
    int count = 0;
    NSString *columns = [NSString string];
    for (NSString *key in keys) {
        count++;
        columns = [columns stringByAppendingString:key];
        if (count < keys.count) {
            columns = [columns stringByAppendingString:@","];
        }
    }
    return columns;
}


@end
