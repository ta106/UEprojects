# UEprojects
A showcase of some of the projects I worked on during my college years
        quarter_df['Week'] = quarter_df.apply(lambda r: 
                                              r['date_'] if r['DOW'] == 6 else
                                              r['date_'] - timedelta(days=(r['DOW']+1) )
                                              
                                              ,axis=1)
